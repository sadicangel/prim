using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax, BinderContext context)
    {
        var left = BindExpression(syntax.Left, context);
        var right = BindExpression(syntax.Right, context);

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

        var functionSymbol = FunctionSymbol.FromOperator(@operator);

        return new BoundBinaryExpression(syntax, left, functionSymbol, right);
    }
}
