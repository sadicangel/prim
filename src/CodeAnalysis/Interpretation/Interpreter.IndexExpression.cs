using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateIndexExpression(BoundIndexExpression node, InterpreterContext context)
    {
        var array = (ArrayValue)EvaluateExpression(node.Expression, context);
        var index = EvaluateExpression(node.Index, context);
        var value = array[index];
        return value;
    }
}
