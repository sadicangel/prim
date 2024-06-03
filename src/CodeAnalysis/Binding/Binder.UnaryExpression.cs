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

        var (expressionKind, operatorKind) = syntax.SyntaxKind switch
        {
            SyntaxKind.UnaryPlusExpression => (BoundKind.UnaryPlusExpression, BoundKind.UnaryPlusOperator),
            SyntaxKind.UnaryMinusExpression => (BoundKind.UnaryMinusExpression, BoundKind.UnaryMinusOperator),
            SyntaxKind.PrefixIncrementExpression => (BoundKind.PrefixIncrementExpression, BoundKind.PrefixIncrementOperator),
            SyntaxKind.PrefixDecrementExpression => (BoundKind.PrefixDecrementExpression, BoundKind.PrefixDecrementOperator),
            SyntaxKind.OnesComplementExpression => (BoundKind.OnesComplementExpression, BoundKind.OnesComplementOperator),
            SyntaxKind.NotExpression => (BoundKind.NotExpression, BoundKind.NotOperator),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };


        var operatorName = SyntaxFacts.GetText(syntax.Operator.SyntaxKind)
            ?? throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.Operator.SyntaxKind}'");
        var operators = operand.Type.GetUnaryOperators(operatorName, operand.Type, PredefinedTypes.Any);
        if (operators is [])
        {
            context.Diagnostics.ReportUndefinedUnaryOperator(syntax.Operator, operand.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        Debug.Assert(operators.Count == 1,
            $"Unexpected result for {nameof(PrimType.GetUnaryOperators)}({operatorName}, {operand.Type}, {operand.Type})");

        var @operator = operators[0];
        var operatorSymbol = new OperatorSymbol(operatorKind, syntax.Operator, @operator);

        return new BoundUnaryExpression(expressionKind, syntax, operatorSymbol, operand);
    }
}
