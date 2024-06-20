using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Evaluator
{
    private static PrimValue EvaluateIdentifierNameExpression(BoundIdentifierNameExpression node, EvaluatorContext context)
    {
        return context.EvaluatedScope.Lookup(node.NameSymbol);
    }
}
