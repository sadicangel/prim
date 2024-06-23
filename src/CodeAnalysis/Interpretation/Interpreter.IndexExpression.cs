using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateIndexExpression(BoundIndexExpression node, InterpreterContext context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        var function = expression.GetOperator(node.OperatorSymbol);
        var index = EvaluateExpression(node.Index, context);
        var value = function.Invoke(index);
        return value;
    }
}
