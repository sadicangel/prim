using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateInvocationExpression(BoundInvocationExpression node, InterpreterContext context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        var function = expression.Get<FunctionValue>(node.FunctionSymbol);
        var arguments = node.Arguments.Select(a => EvaluateExpression(a, context)).ToArray();
        var value = function.Invoke(arguments);
        return value;
    }
}
