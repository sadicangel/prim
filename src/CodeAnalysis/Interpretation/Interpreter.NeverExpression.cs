using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static LiteralValue EvaluateNeverExpression(BoundNeverExpression node, Context context)
    {
        _ = node;
        _ = context;
        return PrimValue.Unit;
    }
}
