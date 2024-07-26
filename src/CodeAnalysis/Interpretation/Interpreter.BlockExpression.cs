using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateBlockExpression(BoundBlockExpression node, Context context)
    {
        PrimValue value = PrimValue.Unit;
        foreach (var expression in node.Expressions)
            value = EvaluateExpression(expression, context);
        return value;
    }
}
