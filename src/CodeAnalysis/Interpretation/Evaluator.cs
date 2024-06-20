using CodeAnalysis.Binding;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
internal static partial class Evaluator
{
    public static PrimValue Evaluate(BoundTree boundTree, EvaluatedScope evaluatedScope)
    {
        var context = new EvaluatorContext(boundTree.Diagnostics, evaluatedScope);
        var value = LiteralValue.Unit as PrimValue;

        foreach (var node in boundTree.CompilationUnit.BoundNodes)
            value = EvaluateNode(node, context);

        return value;
    }
}
