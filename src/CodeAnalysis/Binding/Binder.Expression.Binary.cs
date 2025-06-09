using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax, BindingContext context)
    {
        var left = BindExpression(syntax.Left, context);
        var right = BindExpression(syntax.Right, context);

        var operatorName = Mangler.Mangle(syntax.OperatorToken.SyntaxKind, left.Type, right.Type);
        if (!context.TryLookup<VariableSymbol>(operatorName, out var @operator))
        {
            context.Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken, left.Type.Name, right.Type.Name);

            return new BoundNeverExpression(syntax, context.Module.Never);
        }

        return new BoundBinaryExpression(syntax, left, @operator, right);

        //var containingType = left.Type;
        //var operators = containingType.GetBinaryOperators(syntax.OperatorToken.SyntaxKind, left.Type, right.Type);
        //if (operators is [])
        //{
        //    containingType = right.Type;
        //    operators = containingType.GetBinaryOperators(syntax.OperatorToken.SyntaxKind, left.Type, right.Type);
        //    if (operators is [])
        //    {
        //        context.Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken, left.Type.Name, right.Type.Name);
        //        return new BoundNeverExpression(syntax, context.BoundScope.Never);
        //    }
        //}

        //if (operators is not [var @operator])
        //{
        //    context.Diagnostics.ReportAmbiguousBinaryOperator(syntax.OperatorToken, left.Type.Name, right.Type.Name);
        //    return new BoundNeverExpression(syntax, context.BoundScope.Never);
        //}

        //return new BoundBinaryExpression(syntax, left, @operator, right);
    }
}
