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

        var (expressionKind, operatorKind) = syntax.SyntaxKind switch
        {
            SyntaxKind.AddExpression => (BoundKind.AddExpression, BoundKind.AddOperator),
            SyntaxKind.SubtractExpression => (BoundKind.SubtractExpression, BoundKind.SubtractOperator),
            SyntaxKind.MultiplyExpression => (BoundKind.MultiplyExpression, BoundKind.MultiplyOperator),
            SyntaxKind.DivideExpression => (BoundKind.DivideExpression, BoundKind.DivideOperator),
            SyntaxKind.ModuloExpression => (BoundKind.ModuloExpression, BoundKind.ModuloOperator),
            SyntaxKind.PowerExpression => (BoundKind.PowerExpression, BoundKind.PowerOperator),
            SyntaxKind.LeftShiftExpression => (BoundKind.LeftShiftExpression, BoundKind.LeftShiftOperator),
            SyntaxKind.RightShiftExpression => (BoundKind.RightShiftExpression, BoundKind.RightShiftOperator),
            SyntaxKind.LogicalOrExpression => (BoundKind.LogicalOrExpression, BoundKind.LogicalOrOperator),
            SyntaxKind.LogicalAndExpression => (BoundKind.LogicalAndExpression, BoundKind.LogicalAndOperator),
            SyntaxKind.BitwiseOrExpression => (BoundKind.BitwiseOrExpression, BoundKind.BitwiseOrOperator),
            SyntaxKind.BitwiseAndExpression => (BoundKind.BitwiseAndExpression, BoundKind.BitwiseAndOperator),
            SyntaxKind.ExclusiveOrExpression => (BoundKind.ExclusiveOrExpression, BoundKind.ExclusiveOrOperator),
            SyntaxKind.EqualsExpression => (BoundKind.EqualsExpression, BoundKind.EqualsOperator),
            SyntaxKind.NotEqualsExpression => (BoundKind.NotEqualsExpression, BoundKind.NotEqualsOperator),
            SyntaxKind.LessThanExpression => (BoundKind.LessThanExpression, BoundKind.LessThanOperator),
            SyntaxKind.LessThanOrEqualExpression => (BoundKind.LessThanOrEqualExpression, BoundKind.LessThanOrEqualOperator),
            SyntaxKind.GreaterThanExpression => (BoundKind.GreaterThanExpression, BoundKind.GreaterThanOperator),
            SyntaxKind.GreaterThanOrEqualExpression => (BoundKind.GreaterThanOrEqualExpression, BoundKind.GreaterThanOrEqualOperator),
            SyntaxKind.CoalesceExpression => (BoundKind.CoalesceExpression, BoundKind.CoalesceOperator),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        var operatorName = SyntaxFacts.GetText(syntax.Operator.SyntaxKind)
            ?? throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.Operator.SyntaxKind}'");
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
