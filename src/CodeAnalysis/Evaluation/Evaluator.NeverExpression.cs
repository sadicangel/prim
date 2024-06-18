using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Evaluation.Values;

namespace CodeAnalysis.Evaluation;
partial class Evaluator
{
    private static LiteralValue EvaluateNeverExpression(BoundNeverExpression node, EvaluatorContext context)
    {
        _ = node;
        _ = context;
        return LiteralValue.Unit;
    }
}
