using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Binding;

internal static class BinderExpressionExtensions
{
    extension(Binder binder)
    {
        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            return syntax.SyntaxKind switch
            {
                SyntaxKind.SimpleName =>
                    binder.BindSimpleName((SimpleNameSyntax)syntax),
                SyntaxKind.QualifiedName =>
                    binder.BindQualifiedName((QualifiedNameSyntax)syntax),
                >= SyntaxKind.I8LiteralExpression and <= SyntaxKind.NullLiteralExpression =>
                    binder.BindLiteralExpression((LiteralExpressionSyntax)syntax),
                //SyntaxKind.GroupExpression =>
                //    binder.BindGroupExpression((GroupExpressionSyntax)syntax),
                //SyntaxKind.AssignmentExpression =>
                //    binder.BindAssignmentExpression((AssignmentExpressionSyntax)syntax),
                //SyntaxKind.InitValueExpression =>
                //    binder.BindInitValueExpression((InitValueExpressionSyntax)syntax),
                SyntaxKind.LambdaExpression =>
                    binder.BindLambdaExpression((LambdaExpressionSyntax)syntax),
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
                //SyntaxKind.EmptyExpression =>
                //    binder.BindEmptyExpression((EmptyExpressionSyntax)syntax),
                //SyntaxKind.StatementExpression =>
                //    binder.BindStatementExpression((StatementExpressionSyntax)syntax),
                SyntaxKind.BlockExpression =>
                    binder.BindBlockExpression((BlockExpressionSyntax)syntax),
                //SyntaxKind.ArrayExpression =>
                //    binder.BindArrayInitExpression((ArrayInitExpressionSyntax)syntax),
                //SyntaxKind.StructInitExpression =>
                //    binder.BindStructInitExpression((StructInitExpressionSyntax)syntax),
                //SyntaxKind.MemberAccessExpression =>
                //    binder.BindMemberAccessExpression((MemberAccessExpressionSyntax)syntax),
                //SyntaxKind.IndexExpression =>
                //    binder.BindIndexExpression((IndexExpressionSyntax)syntax),
                //SyntaxKind.InvocationExpression =>
                //    binder.BindInvocationExpression((InvocationExpressionSyntax)syntax),
                //SyntaxKind.ConversionExpression =>
                //    binder.BindConversionExpression((ConversionExpressionSyntax)syntax),
                SyntaxKind.UnaryPlusExpression or
                    SyntaxKind.UnaryMinusExpression or
                    SyntaxKind.OnesComplementExpression or
                    SyntaxKind.NotExpression =>
                    binder.BindUnaryExpression((UnaryExpressionSyntax)syntax),
                SyntaxKind.AddExpression or
                    SyntaxKind.SubtractExpression or
                    SyntaxKind.MultiplyExpression or
                    SyntaxKind.DivideExpression or
                    SyntaxKind.ModuloExpression or
                    SyntaxKind.PowerExpression or
                    SyntaxKind.LeftShiftExpression or
                    SyntaxKind.RightShiftExpression or
                    SyntaxKind.LogicalOrExpression or
                    SyntaxKind.LogicalAndExpression or
                    SyntaxKind.BitwiseOrExpression or
                    SyntaxKind.BitwiseAndExpression or
                    SyntaxKind.ExclusiveOrExpression or
                    SyntaxKind.EqualsExpression or
                    SyntaxKind.NotEqualsExpression or
                    SyntaxKind.LessThanExpression or
                    SyntaxKind.LessThanOrEqualExpression or
                    SyntaxKind.GreaterThanExpression or
                    SyntaxKind.GreaterThanOrEqualExpression or
                    SyntaxKind.CoalesceExpression =>
                    binder.BindBinaryExpression((BinaryExpressionSyntax)syntax),
                //SyntaxKind.IfExpression =>
                //    binder.BindIfElseExpression((IfExpressionSyntax)syntax),
                //SyntaxKind.WhileExpression =>
                //    binder.BindWhileExpression((WhileExpressionSyntax)syntax),
                //SyntaxKind.ContinueExpression =>
                //    binder.BindContinueExpression((ContinueExpressionSyntax)syntax),
                //SyntaxKind.BreakExpression =>
                //    binder.BindBreakExpression((BreakExpressionSyntax)syntax),
                //SyntaxKind.ReturnExpression =>
                //    binder.BindReturnExpression((ReturnExpressionSyntax)syntax),
                _ =>
                    throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            };
        }

        private BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var type = syntax.SyntaxKind switch
            {
                SyntaxKind.I8LiteralExpression => binder.Module.I8,
                SyntaxKind.I16LiteralExpression => binder.Module.I16,
                SyntaxKind.I32LiteralExpression => binder.Module.I32,
                SyntaxKind.I64LiteralExpression => binder.Module.I64,
                SyntaxKind.U8LiteralExpression => binder.Module.U8,
                SyntaxKind.U16LiteralExpression => binder.Module.U16,
                SyntaxKind.U32LiteralExpression => binder.Module.U32,
                SyntaxKind.U64LiteralExpression => binder.Module.U64,
                SyntaxKind.F16LiteralExpression => binder.Module.F16,
                SyntaxKind.F32LiteralExpression => binder.Module.F32,
                SyntaxKind.F64LiteralExpression => binder.Module.F64,
                SyntaxKind.StrLiteralExpression => binder.Module.Str,
                SyntaxKind.TrueLiteralExpression => binder.Module.Bool,
                SyntaxKind.FalseLiteralExpression => binder.Module.Bool,
                SyntaxKind.NullLiteralExpression => binder.Module.Unit,
                _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
            };

            return new BoundLiteralExpression(syntax, type, syntax.InstanceValue);
        }

        private BoundExpression BindSimpleName(SimpleNameSyntax syntax)
        {
            if (!binder.TryLookup<Symbol>(syntax.FullName, out var symbol))
            {
                binder.ReportUndefinedSymbol(syntax.SourceSpan, syntax.FullName);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            return new BoundReference(syntax, symbol);
        }

        private BoundExpression BindQualifiedName(QualifiedNameSyntax syntax)
        {
            if (!binder.TryLookup<Symbol>(syntax.FullName, out var symbol))
            {
                binder.ReportUndefinedSymbol(syntax.SourceSpan, syntax.FullName);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            return new BoundReference(syntax, symbol);
        }

        private BoundLambdaExpression BindLambdaExpression(LambdaExpressionSyntax syntax)
        {
            if (binder is not LambdaBinder lambdaBinder)
            {
                // TODO: Is this an error?
                throw new UnreachableException("Tried to bind a lambda in the wrong scope");
            }

            // TODO: Maybe detect binding errors here and avoid binding the body below?
            lambdaBinder.BindParameters(syntax);

            var body = binder.BindExpression(syntax.Body);

            return new BoundLambdaExpression(syntax, lambdaBinder.LambdaType, [.. lambdaBinder.Parameters], body);
        }

        private BoundModuleDeclaration BindModuleDeclaration(ModuleDeclarationSyntax syntax)
        {
            var module = binder.Module;
            if (syntax.Name.FullName != "<global>" && !binder.TryLookup<ModuleSymbol>(syntax.Name.FullName, out module))
            {
                throw new UnreachableException($"Module {syntax.Name.FullName}' should have already been declared");
            }

            var members = module.Members.Select(member =>
            {
                var (boundNode, diagnostics) = member.Bind();
                binder.AddDiagnostics(diagnostics);
                return boundNode;
            }).ToImmutableArray();

            return new BoundModuleDeclaration(binder.Module, members);
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

        private BoundBlockExpression BindBlockExpression(BlockExpressionSyntax syntax)
        {
            binder = new BlockBinder(binder);

            var types = new HashSet<TypeSymbol>();
            var expressions = ImmutableArray.CreateBuilder<BoundExpression>(syntax.Expressions.Count);
            foreach (var expressionSyntax in syntax.Expressions)
            {
                var expression = binder.BindExpression(expressionSyntax);
                expressions.Add(expression);
                if (expression.CanExitScope)
                    types.Add(expression.Type);
            }

            if (expressions.Count > 0)
                types.Add(expressions[^1].Type);

            var type = types switch
            {
                { Count: 0 } => binder.Module.Unit,
                { Count: 1 } => types.Single(),
                _ when types.Contains(binder.Module.Never) => binder.Module.Never,
                _ => new UnionTypeSymbol(syntax, [.. types], binder.Module)
            };

            return new BoundBlockExpression(syntax, type, expressions.MoveToImmutable());
        }

        private BoundReference? BindOperator(SyntaxToken operatorToken, params ReadOnlySpan<TypeSymbol> types)
        {
            if (types.IsEmpty)
            {
                throw new UnreachableException("Cannot bind an operator with no operand types");
            }

            var operatorName = Mangler.Mangle(operatorToken.SyntaxKind, types);
            foreach (var type in types)
            {
                if (new TypeBinder(type).TryLookup<VariableSymbol>(operatorName, out var @operator))
                {
                    return new BoundReference(operatorToken, @operator);
                }
            }

            // TODO: Report all the types we looked through, not just the first one.
            binder.ReportUndefinedTypeMember(operatorToken.SourceSpan, types[0].FullName, operatorName);

            return null;
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var operand = binder.BindExpression(syntax.Operand);
            if (binder.BindOperator(syntax.OperatorToken, operand.Type) is { } @operator)
            {
                return new BoundUnaryExpression(syntax, @operator, operand);
            }

            return new BoundNeverExpression(syntax, binder.Module.Never);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var left = binder.BindExpression(syntax.Left);
            var right = binder.BindExpression(syntax.Right);
            if (binder.BindOperator(syntax.OperatorToken, left.Type, right.Type) is { } @operator)
            {
                return new BoundBinaryExpression(syntax, left, @operator, right);
            }

            return new BoundNeverExpression(syntax, binder.Module.Never);
        }
    }
}
