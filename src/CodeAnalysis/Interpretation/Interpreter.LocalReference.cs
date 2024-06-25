using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ReferenceValue EvaluateLocalReference(BoundLocalReference node, InterpreterContext context)
    {
        var value = new ReferenceValue(
            node.Type,
            getReferencedValue: () => context.EvaluatedScope.Lookup(node.NameSymbol),
            setReferencedValue: pv => context.EvaluatedScope.Replace(node.NameSymbol, pv));

        return value;
    }
}
