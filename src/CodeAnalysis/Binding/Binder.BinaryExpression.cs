using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax, BinderContext context)
    {
        var left = BindExpression(syntax.Left, context);
        var right = BindExpression(syntax.Right, context);

        var expressionKind = syntax.SyntaxKind switch
        {
            SyntaxKind.AddExpression => BoundKind.AddExpression,
            SyntaxKind.SubtractExpression => BoundKind.SubtractExpression,
            SyntaxKind.MultiplyExpression => BoundKind.MultiplyExpression,
            SyntaxKind.DivideExpression => BoundKind.DivideExpression,
            SyntaxKind.ModuloExpression => BoundKind.ModuloExpression,
            SyntaxKind.PowerExpression => BoundKind.PowerExpression,
            SyntaxKind.LeftShiftExpression => BoundKind.LeftShiftExpression,
            SyntaxKind.RightShiftExpression => BoundKind.RightShiftExpression,
            SyntaxKind.LogicalOrExpression => BoundKind.LogicalOrExpression,
            SyntaxKind.LogicalAndExpression => BoundKind.LogicalAndExpression,
            SyntaxKind.BitwiseOrExpression => BoundKind.BitwiseOrExpression,
            SyntaxKind.BitwiseAndExpression => BoundKind.BitwiseAndExpression,
            SyntaxKind.ExclusiveOrExpression => BoundKind.ExclusiveOrExpression,
            SyntaxKind.EqualsExpression => BoundKind.EqualsExpression,
            SyntaxKind.NotEqualsExpression => BoundKind.NotEqualsExpression,
            SyntaxKind.LessThanExpression => BoundKind.LessThanExpression,
            SyntaxKind.LessThanOrEqualExpression => BoundKind.LessThanOrEqualExpression,
            SyntaxKind.GreaterThanExpression => BoundKind.GreaterThanExpression,
            SyntaxKind.GreaterThanOrEqualExpression => BoundKind.GreaterThanOrEqualExpression,
            SyntaxKind.CoalesceExpression => BoundKind.CoalesceExpression,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        var containingType = left.Type;
        var operators = containingType.GetBinaryOperators(syntax.Operator.SyntaxKind, left.Type, right.Type);
        if (operators is [])
        {
            containingType = right.Type;
            operators = containingType.GetBinaryOperators(syntax.Operator.SyntaxKind, left.Type, right.Type);
            if (operators is [])
            {
                context.Diagnostics.ReportUndefinedBinaryOperator(syntax.Operator.OperatorToken, left.Type.Name, right.Type.Name);
                return new BoundNeverExpression(syntax);
            }
        }

        if (operators is not [var @operator])
        {
            context.Diagnostics.ReportAmbiguousBinaryOperator(syntax.Operator.OperatorToken, left.Type.Name, right.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        var operatorSymbol = new OperatorSymbol(syntax.Operator, @operator);

        return new BoundBinaryExpression(expressionKind, syntax, left, operatorSymbol, right);
    }
}
