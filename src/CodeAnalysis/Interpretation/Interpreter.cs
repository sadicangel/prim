using CodeAnalysis.Binding;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Lowering;

namespace CodeAnalysis.Interpretation;
internal static partial class Interpreter
{
    public static PrimValue Evaluate(BoundTree boundTree, EvaluatedScope evaluatedScope)
    {
        boundTree = Lowerer.Lower(boundTree);

        var context = new InterpreterContext(evaluatedScope);
        var value = PrimValue.Unit as PrimValue;

        foreach (var node in boundTree.CompilationUnit.BoundNodes)
            value = EvaluateNode(node, context);

        return value;
    }
}
