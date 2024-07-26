using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateNopExpression(BoundNopExpression node, InterpreterContext context)
    {
        _ = node;
        return context.LastValue;
    }
}
