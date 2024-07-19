using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateUnaryExpression(BoundUnaryExpression node, InterpreterContext context)
    {
        var operand = EvaluateExpression(node.Operand, context);
        var symbol = node.MethodSymbol.ContainingSymbol == operand.Type
            ? operand
            : EvaluateSymbol(node.MethodSymbol.ContainingSymbol, context);
        var function = symbol.Get<LambdaValue>(node.MethodSymbol);
        var value = function.Invoke(operand);
        return value;
    }
}
