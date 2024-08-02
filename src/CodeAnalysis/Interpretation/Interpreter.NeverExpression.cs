using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static InstanceValue EvaluateNeverExpression(BoundNeverExpression node, Context context)
    {
        _ = node;
        _ = context;
        return context.Unit;
    }
}
