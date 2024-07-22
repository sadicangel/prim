using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateBinaryExpression(BoundBinaryExpression node, InterpreterContext context)
    {
        var left = EvaluateExpression(node.Left, context);
        var right = EvaluateExpression(node.Right, context);
        var target = node.MethodSymbol.ContainingSymbol == left.Type ? left : right;
        var function = target.Get<LambdaValue>(node.MethodSymbol);
        return function.Invoke(left, right);
    }
}
