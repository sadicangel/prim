using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Evaluator
{
    private static LiteralValue EvaluateNeverExpression(BoundNeverExpression node, EvaluatorContext context)
    {
        _ = node;
        _ = context;
        return LiteralValue.Unit;
    }
}
