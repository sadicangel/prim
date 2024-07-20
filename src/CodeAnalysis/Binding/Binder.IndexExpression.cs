using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindIndexExpression(IndexExpressionSyntax syntax, BinderContext context)
    {
        var expression = BindExpression(syntax.Expression, context);

        // TODO: Allow different index operators.
        var @operator = expression.Type.GetOperators(SyntaxKind.BracketOpenBracketCloseToken).SingleOrDefault();

        if (@operator is null)
        {
            context.Diagnostics.ReportUndefinedIndexOperator(syntax.Location, expression.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        // TODO: Actually check where this should be readonly or not.
        // Maybe the indexer should be a property instead?
        var index = Coerce(BindExpression(syntax.Index, context), PredefinedSymbols.I32, context);
        if (index.Type.IsNever)
        {
            return index;
        }

        // TODO: If index is const, check bounds.

        return new BoundIndexReference(syntax, expression, @operator, index, @operator.ReturnType);
    }
}
