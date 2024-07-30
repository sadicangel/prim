using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateUnaryExpression(BoundUnaryExpression node, Context context)
    {
        var operand = EvaluateExpression(node.Operand, context);
        var lambda = operand.Get<LambdaValue>(node.MethodSymbol);
        var value = lambda.Invoke(operand);
        return value;
    }
}
