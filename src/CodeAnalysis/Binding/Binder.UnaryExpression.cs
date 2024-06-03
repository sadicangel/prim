using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax, BindingContext context)
    {
        var operand = BindExpression(syntax.Operand, context);
        if (operand.Type.IsNever)
            return operand;

        var expressionKind = syntax.SyntaxKind switch
        {
            SyntaxKind.UnaryPlusExpression => BoundKind.UnaryPlusExpression,
            SyntaxKind.UnaryMinusExpression => BoundKind.UnaryMinusExpression,
            SyntaxKind.PrefixIncrementExpression => BoundKind.PrefixIncrementExpression,
            SyntaxKind.PrefixDecrementExpression => BoundKind.PrefixDecrementExpression,
            SyntaxKind.OnesComplementExpression => BoundKind.OnesComplementExpression,
            SyntaxKind.NotExpression => BoundKind.NotExpression,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        var operators = operand.Type.GetUnaryOperators(syntax.Operator.SyntaxKind, operand.Type, PredefinedTypes.Any);
        if (operators is [])
        {
            context.Diagnostics.ReportUndefinedUnaryOperator(syntax.Operator.OperatorToken, operand.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        Debug.Assert(operators.Count == 1,
            $"Unexpected result for {nameof(PrimType.GetUnaryOperators)}({syntax.Operator.Text}, {operand.Type}, {operand.Type})");

        var @operator = operators[0];
        var operatorSymbol = new OperatorSymbol(syntax.Operator, @operator);

        return new BoundUnaryExpression(expressionKind, syntax, operatorSymbol, operand);
    }
}
