using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateModuleDeclaration(BoundModuleDeclaration node, Context context)
    {
        throw new NotImplementedException(node.GetType().Name);
    }
}
