using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static VariableValue EvaluateNeverExpression(BoundNeverExpression node, InterpreterContext context)
    {
        _ = node;
        _ = context;
        return VariableValue.Unit;
    }
}
