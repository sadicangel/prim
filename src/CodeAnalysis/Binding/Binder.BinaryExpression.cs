using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax, BindingContext context)
    {
        var left = BindExpression(syntax.Left, context);
        var right = BindExpression(syntax.Right, context);

        var (expressionKind, operatorKind, operatorName) = syntax.SyntaxKind switch
        {
            SyntaxKind.AddExpression => (BoundKind.AddExpression, BoundKind.AddOperator, "operator+"),
            SyntaxKind.SubtractExpression => (BoundKind.SubtractExpression, BoundKind.SubtractOperator, "operator-"),
            SyntaxKind.MultiplyExpression => (BoundKind.MultiplyExpression, BoundKind.MultiplyOperator, "operator*"),
            SyntaxKind.DivideExpression => (BoundKind.DivideExpression, BoundKind.DivideOperator, "operator/"),
            SyntaxKind.ModuloExpression => (BoundKind.ModuloExpression, BoundKind.ModuloOperator, "operator%"),
            SyntaxKind.PowerExpression => (BoundKind.PowerExpression, BoundKind.PowerOperator, "operator**"),
            SyntaxKind.LeftShiftExpression => (BoundKind.LeftShiftExpression, BoundKind.LeftShiftOperator, "operator<<"),
            SyntaxKind.RightShiftExpression => (BoundKind.RightShiftExpression, BoundKind.RightShiftOperator, "operator>>"),
            SyntaxKind.LogicalOrExpression => (BoundKind.LogicalOrExpression, BoundKind.LogicalOrOperator, "operator||"),
            SyntaxKind.LogicalAndExpression => (BoundKind.LogicalAndExpression, BoundKind.LogicalAndOperator, "operator&&"),
            SyntaxKind.BitwiseOrExpression => (BoundKind.BitwiseOrExpression, BoundKind.BitwiseOrOperator, "operator|"),
            SyntaxKind.BitwiseAndExpression => (BoundKind.BitwiseAndExpression, BoundKind.BitwiseAndOperator, "operator&"),
            SyntaxKind.ExclusiveOrExpression => (BoundKind.ExclusiveOrExpression, BoundKind.ExclusiveOrOperator, "operator^"),
            SyntaxKind.EqualsExpression => (BoundKind.EqualsExpression, BoundKind.EqualsOperator, "operator=="),
            SyntaxKind.NotEqualsExpression => (BoundKind.NotEqualsExpression, BoundKind.NotEqualsOperator, "operator!="),
            SyntaxKind.LessThanExpression => (BoundKind.LessThanExpression, BoundKind.LessThanOperator, "operator<"),
            SyntaxKind.LessThanOrEqualExpression => (BoundKind.LessThanOrEqualExpression, BoundKind.LessThanOrEqualOperator, "operator<="),
            SyntaxKind.GreaterThanExpression => (BoundKind.GreaterThanExpression, BoundKind.GreaterThanOperator, "operator>"),
            SyntaxKind.GreaterThanOrEqualExpression => (BoundKind.GreaterThanOrEqualExpression, BoundKind.GreaterThanOrEqualOperator, "operator>="),
            SyntaxKind.CoalesceExpression => (BoundKind.CoalesceExpression, BoundKind.CoalesceOperator, "operator??"),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        var operators = left.Type.GetBinaryOperators(operatorName, left.Type, right.Type, PredefinedTypes.Any);
        if (operators is [])
        {
            context.Diagnostics.ReportUndefinedBinaryOperator(syntax.Operator, left.Type.Name, right.Type.Name);
            return new BoundNeverExpression(syntax);
        }
        if (operators.Count > 1)
        {
            context.Diagnostics.ReportAmbiguousBinaryOperator(syntax.Operator, left.Type.Name, right.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        var @operator = operators[0];
        var operatorSymbol = new OperatorSymbol(operatorKind, syntax.Operator, @operator);

        return new BoundBinaryExpression(expressionKind, syntax, left, operatorSymbol, right);
    }
}
