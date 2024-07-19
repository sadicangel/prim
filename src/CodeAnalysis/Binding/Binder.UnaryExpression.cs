using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax, BinderContext context)
    {
        var operand = BindExpression(syntax.Operand, context);
        if (operand.Type.IsNever)
            return operand;

        var operators = operand.Type.GetUnaryOperators(syntax.OperatorToken.SyntaxKind, operand.Type);
        if (operators is [])
        {
            context.Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken, operand.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        if (operators is not [var @operator])
        {
            // TODO: Is this case ever possible?
            context.Diagnostics.ReportAmbiguousUnaryOperator(syntax.OperatorToken, operand.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        var methodSymbol = MethodSymbol.FromOperator(@operator);

        return new BoundUnaryExpression(syntax, methodSymbol, operand);
    }
}
