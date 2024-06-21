using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateInvocationExpression(BoundInvocationExpression node, InterpreterContext context)
    {
        var function = (FunctionValue)EvaluateExpression(node.Expression, context);
        var arguments = node.Arguments.Select(a => EvaluateExpression(a, context)).ToArray();
        var value = function.Invoke(arguments);
        return value;
    }
}
