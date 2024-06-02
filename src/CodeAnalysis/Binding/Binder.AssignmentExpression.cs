using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax, BindingContext context)
    {
        var left = BindExpression(syntax.Left, context);
        var right = BindExpression(syntax.Right, context);
        var type = PredefinedTypes.Never;
        var boundKind = BoundKind.SimpleAssignmentExpression;

        if (syntax.SyntaxKind is not SyntaxKind.SimpleAssignmentExpression)
        {
            var (expressionKind, operatorKind, operatorName) = syntax.SyntaxKind switch
            {
                SyntaxKind.AddAssignmentExpression => (BoundKind.AddExpression, BoundKind.AddOperator, "operator+"),
                SyntaxKind.SubtractAssignmentExpression => (BoundKind.SubtractExpression, BoundKind.SubtractOperator, "operator-"),
                SyntaxKind.MultiplyAssignmentExpression => (BoundKind.MultiplyExpression, BoundKind.MultiplyOperator, "operator*"),
                SyntaxKind.DivideAssignmentExpression => (BoundKind.DivideExpression, BoundKind.DivideOperator, "operator/"),
                SyntaxKind.ModuloAssignmentExpression => (BoundKind.ModuloExpression, BoundKind.ModuloOperator, "operator%"),
                SyntaxKind.PowerAssignmentExpression => (BoundKind.PowerExpression, BoundKind.PowerOperator, "operator**"),
                SyntaxKind.LeftShiftAssignmentExpression => (BoundKind.LeftShiftExpression, BoundKind.LeftShiftOperator, "operator<<"),
                SyntaxKind.RightShiftAssignmentExpression => (BoundKind.RightShiftExpression, BoundKind.RightShiftOperator, "operator>>"),
                SyntaxKind.OrAssignmentExpression => (BoundKind.LogicalOrExpression, BoundKind.LogicalOrOperator, "operator|"),
                SyntaxKind.AndAssignmentExpression => (BoundKind.LogicalAndExpression, BoundKind.LogicalAndOperator, "operator&"),
                SyntaxKind.ExclusiveOrAssignmentExpression => (BoundKind.ExclusiveOrExpression, BoundKind.ExclusiveOrOperator, "operator^"),
                SyntaxKind.CoalesceAssignmentExpression => (BoundKind.CoalesceExpression, BoundKind.CoalesceOperator, "operator??"),
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

            right = new BoundBinaryExpression(expressionKind, syntax, left, operatorSymbol, right);
        }

        return new BoundAssignmentExpression(boundKind, syntax, type, left, right);
    }
}
