using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ReferenceValue EvaluateGlobalReference(BoundGlobalReference node, Context context)
    {
        var value = new ReferenceValue(
            node.Type,
            getReferencedValue: () => context.EvaluatedScope.LookupGlobal(node.Symbol),
            setReferencedValue: pv => context.EvaluatedScope.ReplaceGlobal(node.Symbol, pv));

        return value;
    }
}
