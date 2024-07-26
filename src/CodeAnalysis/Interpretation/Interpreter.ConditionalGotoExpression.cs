using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateConditionalGotoExpression(BoundConditionalGotoExpression node, InterpreterContext context)
    {
        var condition = EvaluateExpression(node.Condition, context);
        var value = EvaluateExpression(node.Expression, context);
        if (condition.Value is true)
        {
            context.InstructionIndex = context.LabelIndices[node.LabelSymbol];
        }
        return value;
    }
}
