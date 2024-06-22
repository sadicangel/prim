using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax, BinderContext context)
    {
        var operand = BindExpression(syntax.Operand, context);
        if (operand.Type.IsNever)
            return operand;

        var expressionKind = syntax.SyntaxKind switch
        {
            SyntaxKind.UnaryPlusExpression => BoundKind.UnaryPlusExpression,
            SyntaxKind.UnaryMinusExpression => BoundKind.UnaryMinusExpression,
            SyntaxKind.OnesComplementExpression => BoundKind.OnesComplementExpression,
            SyntaxKind.NotExpression => BoundKind.NotExpression,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        var containingType = operand.Type;
        var operators = operand.Type.GetUnaryOperators(syntax.Operator.SyntaxKind, operand.Type);
        if (operators is [])
        {
            context.Diagnostics.ReportUndefinedUnaryOperator(syntax.Operator.OperatorToken, operand.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        if (operators is not [var @operator])
        {
            // TODO: Is this case ever possible?
            context.Diagnostics.ReportAmbiguousUnaryOperator(syntax.Operator.OperatorToken, operand.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        var operatorSymbol = new OperatorSymbol(syntax.Operator, @operator);

        return new BoundUnaryExpression(expressionKind, syntax, operatorSymbol, operand);
    }
}
