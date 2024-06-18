using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Evaluation.Values;

namespace CodeAnalysis.Evaluation;
partial class Evaluator
{
    private static PrimValue EvaluateIdentifierNameExpression(BoundIdentifierNameExpression node, EvaluatorContext context)
    {
        return context.EvaluatedScope.Lookup(node.NameSymbol);
    }
}
