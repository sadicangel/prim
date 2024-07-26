using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundGotoExpression LowerContinueExpression(BoundContinueExpression node, Context context)
    {
        _ = context;
        return new BoundGotoExpression(node.Syntax, node.LabelSymbol, node.Expression);
    }
}
