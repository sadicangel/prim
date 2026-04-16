using System.Collections.Immutable;
using System.Diagnostics;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.References;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
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
                >= SyntaxKind.I8LiteralExpression and <= SyntaxKind.NullLiteralExpression =>
                    binder.BindLiteralExpression((LiteralExpressionSyntax)syntax),
                //SyntaxKind.GroupExpression =>
                //    binder.BindGroupExpression((GroupExpressionSyntax)syntax),
                SyntaxKind.AssignmentExpression =>
                    binder.BindAssignmentExpression((AssignmentExpressionSyntax)syntax),
                //SyntaxKind.StructExpression =>
                //    binder.BindStructExpression((StructExpressionSyntax)syntax),
                SyntaxKind.LambdaExpression =>
                    binder.BindLambdaExpression((LambdaExpressionSyntax)syntax),
                SyntaxKind.ArrayExpression =>
                    binder.BindArrayExpression((ArrayExpressionSyntax)syntax),
                SyntaxKind.StructExpression =>
                    binder.BindStructExpression((StructExpressionSyntax)syntax),
                SyntaxKind.PropertyAccessExpression =>
                    binder.BindPropertyAccessExpression((PropertyAccessExpressionSyntax)syntax),
                SyntaxKind.ElementAccessExpression =>
                    binder.BindElementAccessExpression((ElementAccessExpressionSyntax)syntax),
                SyntaxKind.InvocationExpression =>
                    binder.BindCallExpression((CallExpressionSyntax)syntax),
                SyntaxKind.ConversionExpression =>
                    binder.BindConversionExpression((ConversionExpressionSyntax)syntax),
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
                SyntaxKind.SimpleName =>
                    binder.BindSimpleName((SimpleNameSyntax)syntax),
                SyntaxKind.QualifiedName =>
                    binder.BindQualifiedName((QualifiedNameSyntax)syntax),
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

        private static BoundReference CreateReference(SyntaxNode syntax, Symbol symbol) => symbol switch
        {
            PropertySymbol propertySymbol => new BoundPropertyReference(syntax, null, propertySymbol),
            VariableSymbol variableSymbol => new BoundVariableReference(syntax, variableSymbol),
            OperatorSymbol operatorSymbol => new BoundOperatorReference(syntax, operatorSymbol),
            _ => throw new ArgumentOutOfRangeException(nameof(symbol))
        };

        private BoundExpression BindSimpleName(SimpleNameSyntax syntax)
        {
            if (!binder.TryLookup<Symbol>(syntax.FullName, out var symbol))
            {
                binder.ReportUndefinedSymbol(syntax.SourceSpan, syntax.FullName);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            return Binder.CreateReference(syntax, symbol);
        }

        private BoundExpression BindQualifiedName(QualifiedNameSyntax syntax)
        {
            if (!binder.TryLookup<Symbol>(syntax.FullName, out var symbol))
            {
                binder.ReportUndefinedSymbol(syntax.SourceSpan, syntax.FullName);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            return Binder.CreateReference(syntax, symbol);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var left = binder.BindExpression(syntax.Left) as BoundReference;
            if (left is null)
            {
                binder.ReportInvalidAssignment(syntax.Left.SourceSpan);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            if (left.IsReadOnly)
            {
                binder.ReportReadOnlyAssignment(syntax.Left.SourceSpan, left.Symbol.Name);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            var right = binder.BindExpression(syntax.Right);
            right = binder.Convert(right, left.Type);
            return new BoundAssignmentExpression(syntax, left, right);
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

            var body = binder.BindStatement(syntax.Body);

            return new BoundLambdaExpression(syntax, lambdaBinder.LambdaType, [.. lambdaBinder.Parameters], body);
        }

        private BoundArrayExpression BindArrayExpression(ArrayExpressionSyntax syntax)
        {
            TypeSymbol elementType = binder.Module.Unknown;
            var elements = ImmutableArray.CreateBuilder<BoundExpression>(syntax.Elements.Count);
            foreach (var elementSyntax in syntax.Elements)
            {
                var element = binder.BindExpression(elementSyntax);
                elements.Add(element);
                if (!element.Type.MapsToUnknown)
                {
                    if (elementType.MapsToUnknown)
                    {
                        elementType = element.Type;
                    }
                    else if (element.Type != elementType)
                    {
                        binder.ReportArrayElementTypeMismatch(elementSyntax.SourceSpan /*, element.Type.Name, elementType.Name*/);
                    }
                }
            }

            if (elementType.MapsToUnknown)
            {
                binder.ReportInvalidImplicitType(syntax.SourceSpan, "array");
            }

            return new BoundArrayExpression(syntax, new ArrayTypeSymbol(syntax, elementType, null, binder.Module), elements.MoveToImmutable());
        }

        private BoundExpression BindStructExpression(StructExpressionSyntax syntax)
        {
            if (!binder.TryLookup<StructTypeSymbol>(syntax.StructName.FullName, out var structType))
            {
                binder.ReportUndefinedSymbol(syntax.StructName.SourceSpan, syntax.StructName.FullName);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            var typeBinder = new TypeBinder(structType, binder);
            var properties = ImmutableArray.CreateBuilder<BoundPropertyExpression>(syntax.Properties.Count);
            foreach (var propertySyntax in syntax.Properties)
            {
                if (typeBinder.BindPropertyExpression(propertySyntax) is not BoundPropertyExpression property)
                {
                    return new BoundNeverExpression(propertySyntax, binder.Module.Never);
                }

                properties.Add(property);
            }

            return new BoundStructExpression(syntax, structType, properties.MoveToImmutable());
        }

        private BoundExpression BindPropertyExpression(PropertyExpressionSyntax syntax)
        {
            if (!binder.TryLookup<PropertySymbol>(syntax.PropertyName.FullName, out var property))
            {
                binder.ReportUndefinedTypeMember(syntax.SourceSpan, ((TypeBinder)binder).Type.FullName, syntax.PropertyName.FullName);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            var expression = binder.BindExpression(syntax.Value);

            expression = binder.Convert(expression, property.Type);
            return new BoundPropertyExpression(syntax, property, expression);
        }

        private BoundOperatorReference? BindOperator(TypeSymbol containingType, SyntaxToken operatorToken, params ReadOnlySpan<TypeSymbol> operandTypes)
        {
            var operatorKind = operatorToken.SyntaxKind.GetOperatorKind(operandTypes.Length);
            var operatorName = operatorKind.GetOperatorName(operandTypes);
            binder = new TypeBinder(containingType);
            if (binder.TryLookup<OperatorSymbol>(operatorName, out var @operator))
            {
                return new BoundOperatorReference(operatorToken, @operator);
            }

            return null;
        }

        private BoundExpression BindPropertyAccessExpression(PropertyAccessExpressionSyntax syntax)
        {
            var receiver = binder.BindExpression(syntax.Receiver);
            var typeBinder = new TypeBinder(receiver.Type);
            if (typeBinder.TryLookup<PropertySymbol>(syntax.PropertyName.FullName, out var property))
            {
                return new BoundPropertyReference(syntax, receiver, property);
            }

            binder.ReportUndefinedTypeMember(syntax.PropertyName.SourceSpan, receiver.Type.FullName, syntax.PropertyName.FullName);
            return new BoundNeverExpression(syntax, binder.Module.Never);
        }

        private BoundExpression BindElementAccessExpression(ElementAccessExpressionSyntax syntax)
        {
            var receiver = binder.BindExpression(syntax.Receiver);
            var index = binder.BindExpression(syntax.Index);

            var typeBinder = new TypeBinder(receiver.Type);
            if (typeBinder.TryLookup<IndexerSymbol>($"[{index.Type.Name}]", out var indexer))
            {
                index = binder.Convert(index, indexer.IndexType);
                return new BoundElementReference(syntax, receiver, indexer, index);
            }

            binder.ReportUndefinedTypeMember(syntax.Index.SourceSpan, receiver.Type.FullName, Mangler.Mangle(SyntaxKind.BracketOpenBracketCloseToken, index.Type));
            return new BoundNeverExpression(syntax, binder.Module.Never);
        }

        private BoundExpression BindCallExpression(CallExpressionSyntax syntax)
        {
            var callee = binder.BindExpression(syntax.Callee);
            // TODO: Support 'call' operator.
            // In this case the callee itself is the lambda, so it would probably not make sense to define an operator on it,
            // but we might want to support some kind of "call" operator in the future.
            if (callee.Type is not LambdaTypeSymbol lambdaType)
            {
                binder.ReportUndefinedInvocationOperator(callee.Syntax.SourceSpan, callee.Type.Name);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            if (lambdaType.Parameters.Length != syntax.Arguments.Count)
            {
                binder.ReportArgumentCountMismatch(syntax.SourceSpan /*, lambdaType.ParameterTypes.Length*/, syntax.Arguments.Count);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            var arguments = syntax.Arguments
                .Select(binder.BindExpression)
                .Select((argument, index) => binder.Convert(argument, lambdaType.Parameters[index]))
                .ToImmutableArray();

            return new BoundCallExpression(syntax, callee, arguments, lambdaType.ReturnType);
        }

        private BoundExpression BindConversionExpression(ConversionExpressionSyntax syntax)
        {
            var expression = binder.BindExpression(syntax.Expression);
            var targetType = binder.BindType(syntax.Type);

            return binder.Convert(expression, targetType, isExplicit: true);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var operand = binder.BindExpression(syntax.Operand);
            if (binder.BindOperator(operand.Type, syntax.OperatorToken, operand.Type) is { } @operator)
            {
                return new BoundUnaryExpression(syntax, @operator, operand);
            }

            binder.ReportUndefinedTypeMember(syntax.OperatorToken.SourceSpan, operand.Type.FullName, Mangler.Mangle(syntax.SyntaxKind, operand.Type));

            return new BoundNeverExpression(syntax, binder.Module.Never);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var left = binder.BindExpression(syntax.Left);
            var right = binder.BindExpression(syntax.Right);
            var @operator = binder.BindOperator(left.Type, syntax.OperatorToken, left.Type, right.Type)
                ?? binder.BindOperator(right.Type, syntax.OperatorToken, left.Type, right.Type);
            if (@operator is not null)
            {
                return new BoundBinaryExpression(syntax, left, @operator, right);
            }

            binder.ReportUndefinedTypeMember(syntax.OperatorToken.SourceSpan, left.Type.FullName, Mangler.Mangle(syntax.SyntaxKind, left.Type, right.Type));
            if (left.Type != right.Type)
                binder.ReportUndefinedTypeMember(syntax.OperatorToken.SourceSpan, right.Type.FullName, Mangler.Mangle(syntax.SyntaxKind, left.Type, right.Type));

            return new BoundNeverExpression(syntax, binder.Module.Never);
        }
    }
}
