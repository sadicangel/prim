using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundNeverExpression LowerNeverExpression(BoundNeverExpression node) => node;
}
