using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateIfElseExpression(BoundIfElseExpression node, InterpreterContext context)
    {
        var condition = EvaluateExpression(node.Condition, context);
        var value = (bool)condition.Value
            ? EvaluateExpression(node.Then, context)
            : EvaluateExpression(node.Else, context);
        return value;
    }
}
