using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.ControlFlow;
using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal static class BinderStatementExtensions
{
    extension(Binder binder)
    {
        public BoundExpression BindStatement(StatementSyntax syntax)
        {
            return syntax.Kind switch
            {
                SyntaxKind.GlobalDeclaration => binder.BindDeclaration((GlobalDeclarationSyntax)syntax),
                SyntaxKind.LocalDeclaration => binder.BindDeclaration((LocalDeclarationSyntax)syntax),
                SyntaxKind.ExpressionStatement => binder.BindExpressionStatement((ExpressionStatementSyntax)syntax),
                SyntaxKind.EmptyStatement => new BoundNopExpression(syntax, binder.Module.Unit),
                _ => throw new NotImplementedException(syntax.Kind.ToString()),
            };
        }

        private BoundExpression BindExpressionStatement(ExpressionStatementSyntax syntax) => binder.BindExpression(syntax.Expression);

        private BoundExpression BindDeclaration(DeclarationSyntax syntax)
        {
            if (syntax is GlobalDeclarationSyntax { Initializer: ModuleExpressionSyntax } globalModule)
            {
                var module = binder.ResolveModule(globalModule.Name)
                    ?? throw new UnreachableException($"Module '{globalModule.Name.FullName}' should have already been declared");

                return binder.BindModuleDeclaration(module);
            }

            if (syntax is GlobalDeclarationSyntax { Initializer: TypeExpressionSyntax } globalStruct)
            {
                if (!binder.TryLookup<StructTypeSymbol>(globalStruct.Name.FullName, out var structType))
                    throw new UnreachableException($"Struct '{globalStruct.Name.FullName}' should have already been declared");

                return binder.BindStructDeclaration(structType);
            }

            return binder.BindVariableDeclaration(syntax);
        }

        private ModuleSymbol? ResolveModule(NameSyntax name)
        {
            var module = binder.Module;
            foreach (var part in name.Name)
            {
                if (!module.TryLookup<ModuleSymbol>(part, out var childModule))
                    return null;

                module = childModule;
            }

            return module;
        }

        private BoundModuleDeclaration BindModuleDeclaration(ModuleSymbol module)
        {
            var members = module.Members.Select(member =>
            {
                var (boundNode, diagnostics) = member.BindDeclared();
                binder.AddDiagnostics(diagnostics);
                return boundNode;
            }).ToArray();

            return new BoundModuleDeclaration(module, [.. members]);
        }

        private BoundStructDeclaration BindStructDeclaration(StructTypeSymbol structType)
        {
            var properties = structType.Members.OfType<PropertySymbol>()
                .Select(property => binder.BindPropertyDeclaration(property))
                .ToArray();

            return new BoundStructDeclaration(structType, [.. properties]);
        }

        private BoundPropertyDeclaration BindPropertyDeclaration(PropertySymbol property)
        {
            var syntax = (LocalDeclarationSyntax)property.Syntax;
            var initializer = default(BoundExpression);
            if (syntax.Initializer is not null)
            {
                initializer = binder.BindExpression(syntax.Initializer);
                initializer = binder.Convert(initializer, property.Type);
            }

            return new BoundPropertyDeclaration(property, initializer);
        }

        private BoundVariableDeclaration BindVariableDeclaration(DeclarationSyntax syntax)
        {
            var name = syntax switch
            {
                GlobalDeclarationSyntax global => global.Name,
                LocalDeclarationSyntax local => local.Name,
                _ => throw new UnreachableException()
            };
            var typeSyntax = syntax switch
            {
                GlobalDeclarationSyntax global => global.Type,
                LocalDeclarationSyntax local => local.Type,
                _ => null
            };
            var initializer = syntax switch
            {
                GlobalDeclarationSyntax global => global.Initializer,
                LocalDeclarationSyntax local => local.Initializer,
                _ => null
            };
            var explicitType = typeSyntax is not null ? binder.BindType(typeSyntax) : null;
            var variable = default(VariableSymbol);

            if (syntax is GlobalDeclarationSyntax && binder.TryLookup<VariableSymbol>(name.FullName, out var existing))
            {
                variable = existing;
            }
            else
            {
                var variableType = explicitType ?? binder.Module.Unknown;
                variable = new VariableSymbol(
                    syntax,
                    name.FullName,
                    variableType,
                    binder.Module,
                    syntax.IsReadOnly ? Modifiers.ReadOnly : Modifiers.None);

                if (!binder.TryDeclare(variable))
                    binder.ReportSymbolRedeclaration(syntax.SourceSpan, variable.Name);
            }

            if (initializer is not null)
            {
                var initializerBinder = binder;
                if ((explicitType ?? variable.Type) is LambdaTypeSymbol lambdaType)
                    initializerBinder = new LambdaBinder(lambdaType, binder);

                var expression = initializerBinder.BindExpression(initializer);

                if (explicitType is not null || !variable.Type.MapsToUnknown)
                {
                    expression = binder.Convert(expression, variable.Type);
                    return new BoundVariableDeclaration(syntax, variable, expression);
                }

                if (expression.Type.MapsToUnknown)
                {
                    binder.ReportInvalidImplicitType(syntax.SourceSpan, expression.Type.Name);
                    return new BoundVariableDeclaration(syntax, variable, expression);
                }

                SetType(variable, expression.Type);
                return new BoundVariableDeclaration(syntax, variable, expression);
            }

            if (variable.Type.MapsToUnit)
            {
                var expression = new BoundLiteralExpression(SyntaxToken.CreateSynthetic(SyntaxKind.NullKeyword), binder.Module.Unit, Unit.Value);
                return new BoundVariableDeclaration(syntax, variable, expression);
            }

            binder.ReportUninitializedVariable(syntax.SourceSpan, variable.Name);
            return new BoundVariableDeclaration(syntax, variable, new BoundNeverExpression(syntax, binder.Module.Never));
        }

        public BoundIfElseExpression BindIfElseExpression(IfElseExpressionSyntax syntax)
        {
            var condition = binder.BindExpression(syntax.Condition);
            condition = binder.Convert(condition, binder.Module.Bool);

            var then = binder.BindExpression(syntax.Then);
            if (syntax.ElseClause is null)
                return new BoundIfElseExpression(syntax, condition, then);

            var @else = binder.BindExpression(syntax.ElseClause.Else);
            var type = then.Type == @else.Type ? then.Type : binder.Module.Never;
            if (type.IsNever)
                binder.ReportInvalidConversion(syntax.SourceSpan, then.Type.Name, @else.Type.Name);

            return new BoundIfElseExpression(syntax, condition, then, @else, type);
        }

        public BoundWhileExpression BindWhileExpression(WhileExpressionSyntax syntax)
        {
            var condition = binder.BindExpression(syntax.Condition);
            condition = binder.Convert(condition, binder.Module.Bool);
            var body = new LoopBinder(binder).BindExpression(syntax.Body);
            return new BoundWhileExpression(syntax, condition, body);
        }

        public BoundExpression BindBreakExpression(BreakExpressionSyntax syntax)
        {
            if (!binder.IsInsideLoop())
            {
                binder.ReportInvalidBreakOrContinue(syntax.SourceSpan);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            var expression = syntax.Expression is not null
                ? binder.BindExpression(syntax.Expression)
                : binder.CreateUnitExpression(syntax.BreakKeyword);

            return new BoundBreakExpression(syntax, expression);
        }

        public BoundExpression BindContinueExpression(ContinueExpressionSyntax syntax)
        {
            if (!binder.IsInsideLoop())
            {
                binder.ReportInvalidBreakOrContinue(syntax.SourceSpan);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            return new BoundContinueExpression(syntax, binder.Module.Unit);
        }

        public BoundExpression BindReturnExpression(ReturnExpressionSyntax syntax)
        {
            var lambdaType = binder.GetEnclosingLambdaType();
            if (lambdaType is null)
            {
                binder.ReportInvalidReturn(syntax.SourceSpan);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            var expression = syntax.Expression is not null
                ? binder.BindExpression(syntax.Expression)
                : binder.CreateUnitExpression(syntax.ReturnKeyword);
            expression = binder.Convert(expression, lambdaType.ReturnType);

            return new BoundReturnExpression(syntax, expression);
        }

        private bool IsInsideLoop()
        {
            for (var current = binder; current is not null; current = current.Parent)
                if (current is LoopBinder)
                    return true;

            return false;
        }

        private LambdaTypeSymbol? GetEnclosingLambdaType()
        {
            for (var current = binder; current is not null; current = current.Parent)
                if (current is LambdaBinder lambdaBinder)
                    return lambdaBinder.LambdaType;

            return null;
        }

        private BoundLiteralExpression CreateUnitExpression(SyntaxNode syntax) =>
            new(syntax, binder.Module.Unit, Unit.Value);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(TypeSymbol.Type)}")]
        private static extern void SetType(Symbol symbol, TypeSymbol type);
    }
}
