using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateConditionalGotoExpression(BoundConditionalGotoExpression node, Context context)
    {
        var condition = EvaluateExpression(node.Condition, context);
        if ((bool)condition.Value == node.JumpTrue)
        {
            var value = EvaluateExpression(node.Expression, context);
            context.InstructionIndex = context.LabelIndices[node.LabelSymbol];
            return value;
        }
        return context.LastValue;
    }
}
