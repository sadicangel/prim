using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundNeverExpression BindEmptyExpression(EmptyExpressionSyntax syntax, Context context)
    {
        _ = context;
        return new BoundNeverExpression(syntax);
    }
}
