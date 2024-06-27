using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateWhileExpression(BoundWhileExpression node, InterpreterContext context)
    {
        PrimValue value = PrimValue.Unit;
        while ((bool)EvaluateExpression(node.Condition, context).Value)
            value = EvaluateExpression(node.Body, context);
        return value;
    }
}
