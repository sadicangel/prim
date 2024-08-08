using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateInvocationExpression(BoundInvocationExpression node, Context context)
    {
        using (context.PushScope())
        {
            var expression = EvaluateExpression(node.Expression, context);
            var function = expression.Get<LambdaValue>(node.OperatorSymbol);
            var arguments = node.Arguments.Select(a => EvaluateExpression(a, context)).ToArray();
            var value = function.Invoke(arguments);
            return value;
        }
    }
}
