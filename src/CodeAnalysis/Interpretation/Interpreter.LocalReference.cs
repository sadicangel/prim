using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ReferenceValue EvaluateLocalReference(BoundLocalReference node, Context context)
    {
        var value = new ReferenceValue(
            node.Type,
            getReferencedValue: () => context.EvaluatedScope.LookupLocal(node.Symbol),
            setReferencedValue: pv => context.EvaluatedScope.ReplaceLocal(node.Symbol, pv));

        return value;
    }
}
