using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateBinaryExpression(BoundBinaryExpression node, Context context)
    {
        var left = EvaluateExpression(node.Left, context);
        var right = EvaluateExpression(node.Right, context);
        var target = node.OperatorSymbol.ContainingType == left.Type ? left : right;
        var function = target.Get<LambdaValue>(node.OperatorSymbol);
        var value = function.Invoke(left, right);
        return value;
    }
}
