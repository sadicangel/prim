using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.ControlFlow;
using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;
using CodeAnalysis.Syntax.Statements;

namespace CodeAnalysis.Binding;

internal static class BinderStatementExtensions
{
    extension(Binder binder)
    {
        public BoundExpression BindStatement(StatementSyntax syntax)
        {
            return syntax.SyntaxKind switch
            {
                SyntaxKind.BlockStatement =>
                    binder.BindBlockExpression((BlockStatementSyntax)syntax),
                SyntaxKind.ExpressionStatement =>
                    binder.BindExpressionStatement((ExpressionStatementSyntax)syntax),
                SyntaxKind.EmptyStatement =>
                    binder.BindEmptyExpression((EmptyStatementSyntax)syntax),
                SyntaxKind.IfElseStatement =>
                    binder.BindIfElseExpression((IfElseStatementSyntax)syntax),
                SyntaxKind.WhileStatement =>
                    binder.BindWhileExpression((WhileStatementSyntax)syntax),
                SyntaxKind.ContinueStatement =>
                    binder.BindContinueExpression((ContinueStatementSyntax)syntax),
                SyntaxKind.BreakStatement =>
                    binder.BindBreakExpression((BreakStatementSyntax)syntax),
                SyntaxKind.ReturnStatement =>
                    binder.BindReturnExpression((ReturnStatementSyntax)syntax),
                SyntaxKind.VariableDeclaration =>
                    binder.BindVariableDeclaration((VariableDeclarationSyntax)syntax),
                SyntaxKind.ModuleDeclaration =>
                    binder.BindModuleDeclaration((ModuleDeclarationSyntax)syntax),
                SyntaxKind.StructDeclaration =>
                    binder.BindStructDeclaration((StructDeclarationSyntax)syntax),
                SyntaxKind.PropertyDeclaration =>
                    binder.BindPropertyDeclaration((PropertyDeclarationSyntax)syntax),
                //SyntaxKind.LocalDeclaration =>
                //    binder.BindLocalDeclaration((LocalDeclarationSyntax)syntax),
                _ =>
                    throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            };
        }

        private BoundBlockExpression BindBlockExpression(BlockStatementSyntax syntax)
        {
            binder = new BlockBinder(binder);

            var types = new HashSet<TypeSymbol>();
            var statements = ImmutableArray.CreateBuilder<BoundExpression>(syntax.Statements.Count);
            foreach (var statementSyntax in syntax.Statements)
            {
                var expression = binder.BindStatement(statementSyntax);
                statements.Add(expression);
                if (expression.CanExitScope)
                    types.Add(expression.Type);
            }

            if (statements.Count > 0)
                types.Add(statements[^1].Type);

            var type = types switch
            {
                { Count: 0 } => binder.Module.Unit,
                { Count: 1 } => types.Single(),
                _ when types.Contains(binder.Module.Never) => binder.Module.Never,
                _ => new UnionTypeSymbol(syntax, [.. types], binder.Module)
            };

            return new BoundBlockExpression(syntax, type, statements.MoveToImmutable());
        }

        private BoundExpression BindExpressionStatement(ExpressionStatementSyntax syntax) => binder.BindExpression(syntax.Expression);

        private BoundNopExpression BindEmptyExpression(EmptyStatementSyntax syntax) => new(syntax, binder.Module.Never);

        public BoundIfElseExpression BindIfElseExpression(IfElseStatementSyntax syntax)
        {
            var condition = binder.BindExpression(syntax.Condition);
            condition = binder.Convert(condition, binder.Module.Bool);

            var then = binder.BindStatement(syntax.Then);
            if (syntax.ElseClause is null)
            {
                return new BoundIfElseExpression(syntax, condition, then);
            }

            var @else = binder.BindStatement(syntax.ElseClause.Else);

            // TODO: We should support implicit conversions here and provide a better error message when the types are incompatible.
            var type = then.Type == @else.Type ? then.Type : binder.Module.Never;
            if (type.IsNever)
            {
                binder.ReportInvalidConversion(syntax.SourceSpan, then.Type.Name, @else.Type.Name);
            }

            return new BoundIfElseExpression(syntax, condition, then, @else, type);
        }

        public BoundWhileExpression BindWhileExpression(WhileStatementSyntax syntax)
        {
            var condition = binder.BindExpression(syntax.Condition);
            condition = binder.Convert(condition, binder.Module.Bool);

            var body = new LoopBinder(binder).BindStatement(syntax.Body);
            return new BoundWhileExpression(syntax, condition, body);
        }

        public BoundExpression BindBreakExpression(BreakStatementSyntax syntax)
        {
            if (!binder.IsInsideLoop())
            {
                binder.ReportInvalidBreakOrContinue(syntax.SourceSpan);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            var expression = syntax.Expression is not null
                ? binder.BindExpression(syntax.Expression)
                : binder.CreateUnitExpression(syntax.SemicolonToken ?? syntax.BreakKeyword);

            return new BoundBreakExpression(syntax, expression);
        }

        public BoundExpression BindContinueExpression(ContinueStatementSyntax syntax)
        {
            if (!binder.IsInsideLoop())
            {
                binder.ReportInvalidBreakOrContinue(syntax.SourceSpan);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            return new BoundContinueExpression(syntax, binder.Module.Unit);
        }

        public BoundExpression BindReturnExpression(ReturnStatementSyntax syntax)
        {
            var lambdaType = binder.GetEnclosingLambdaType();
            if (lambdaType is null)
            {
                binder.ReportInvalidReturn(syntax.SourceSpan);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            var expression = syntax.Expression is not null
                ? binder.BindExpression(syntax.Expression)
                : binder.CreateUnitExpression(syntax.SemicolonToken ?? syntax.ReturnKeyword);
            expression = binder.Convert(expression, lambdaType.ReturnType);

            return new BoundReturnExpression(syntax, expression);
        }

        private bool IsInsideLoop()
        {
            for (var current = binder; current is not null; current = current.Parent)
            {
                if (current is LoopBinder)
                {
                    return true;
                }
            }

            return false;
        }

        private LambdaTypeSymbol? GetEnclosingLambdaType()
        {
            for (var current = binder; current is not null; current = current.Parent)
            {
                if (current is LambdaBinder lambdaBinder)
                {
                    return lambdaBinder.LambdaType;
                }
            }

            return null;
        }

        private BoundLiteralExpression CreateUnitExpression(SyntaxNode syntax) =>
            new(syntax, binder.Module.Unit, Unit.Value);


        private BoundModuleDeclaration BindModuleDeclaration(ModuleDeclarationSyntax syntax)
        {
            var module = binder.Module;
            if (syntax.Name.FullName != "<global>" && !binder.TryLookup<ModuleSymbol>(syntax.Name.FullName, out module))
            {
                throw new UnreachableException($"Module {syntax.Name.FullName}' should have already been declared");
            }

            var members = module.Members.Select(member =>
            {
                var (boundNode, diagnostics) = member.BindDeclared();
                binder.AddDiagnostics(diagnostics);
                return boundNode;
            }).ToImmutableArray();

            return new BoundModuleDeclaration(module, members);
        }

        private BoundStructDeclaration BindStructDeclaration(StructDeclarationSyntax syntax)
        {
            if (!binder.TryLookup<StructTypeSymbol>(syntax.Name.FullName, out var structType))
            {
                throw new UnreachableException($"Struct '{syntax.Name.FullName}' should have already been declared");
            }

            binder = new TypeBinder(structType, binder);
            var properties = structType.Members
                .Select(member => binder.BindPropertyDeclaration((PropertyDeclarationSyntax)member.Syntax))
                .ToImmutableArray();

            return new BoundStructDeclaration(structType, properties);
        }

        private BoundPropertyDeclaration BindPropertyDeclaration(PropertyDeclarationSyntax syntax)
        {
            if (!binder.TryLookup<PropertySymbol>(syntax.Name.FullName, out var property))
            {
                throw new UnreachableException($"Property '{syntax.Name.FullName}' should have already been declared");
            }

            var initializer = default(BoundExpression);
            if (syntax.InitClause is not null)
            {
                initializer = binder.BindExpression(syntax.InitClause.Expression);

                if (initializer.Type.MapsToUnknown)
                {
                    binder.ReportInvalidImplicitType(syntax.SourceSpan, initializer.Type.Name);
                }
            }

            return new BoundPropertyDeclaration(property, initializer);
        }

        private BoundVariableDeclaration BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            var variableType = syntax.TypeClause is not null ? binder.BindType(syntax.TypeClause.Type) : binder.Module.Unknown;
            var modifiers = syntax.BindingKeyword.SyntaxKind is SyntaxKind.LetKeyword ? Modifiers.ReadOnly : Modifiers.None;

            var variable = new VariableSymbol(syntax, syntax.Name.FullName, variableType, binder.Module, modifiers);
            if (!binder.TryDeclare(variable))
            {
                binder.ReportSymbolRedeclaration(syntax.SourceSpan, variable.Name);
            }

            if (syntax.InitClause?.Expression is { } initExpression)
            {
                if (variableType is LambdaTypeSymbol lambdaType)
                {
                    binder = new LambdaBinder(lambdaType, binder);
                }

                var expression = binder.BindExpression(initExpression);
                if (!variable.Type.MapsToUnknown)
                {
                    expression = binder.Convert(expression, variable.Type);
                    return new BoundVariableDeclaration(syntax, variable, expression);
                }

                if (expression.Type.MapsToUnknown)
                {
                    binder.ReportInvalidImplicitType(syntax.SourceSpan, expression.Type.Name);
                }

                // We've inferred the symbol type, we can replace it with some magic.
                SetType(variable, expression.Type);

                return new BoundVariableDeclaration(syntax, variable, expression);

                [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(TypeSymbol.Type)}")]
                static extern void SetType(Symbol symbol, TypeSymbol type);
            }

            if (variable.Type.MapsToUnit)
            {
                var expression = new BoundLiteralExpression(SyntaxToken.CreateSynthetic(SyntaxKind.NullKeyword), binder.Module.Unit, Unit.Value);
                return new BoundVariableDeclaration(syntax, variable, expression);
            }

            binder.ReportUninitializedVariable(syntax.SourceSpan, variable.Name);
            return new BoundVariableDeclaration(syntax, variable, new BoundNeverExpression(syntax, binder.Module.Never));
        }
    }
}
