using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateLabelDeclaration(BoundLabelDeclaration node, Context context)
    {
        _ = node;
        return context.LastValue;
    }
}
