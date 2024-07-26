using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Lowering;

namespace CodeAnalysis.Interpretation;
internal static partial class Interpreter
{
    public static PrimValue Evaluate(BoundTree boundTree, EvaluatedScope evaluatedScope)
    {
        boundTree = Lowerer.Lower(boundTree);

        var labelIndices = new Dictionary<LabelSymbol, int>();
        for (var i = 0; i < boundTree.CompilationUnit.BoundNodes.Count; ++i)
            if (boundTree.CompilationUnit.BoundNodes[i] is BoundLabelDeclaration labelStatement)
                labelIndices[labelStatement.LabelSymbol] = i;

        var context = new Context(evaluatedScope, labelIndices);

        while (context.InstructionIndex < boundTree.CompilationUnit.BoundNodes.Count)
        {
            var node = boundTree.CompilationUnit.BoundNodes[context.InstructionIndex];
            context.LastValue = EvaluateNode(node, context);
            ++context.InstructionIndex;
        }

        return context.LastValue;
    }
}
