using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateUnaryExpression(BoundUnaryExpression node, InterpreterContext context)
    {
        var operand = EvaluateExpression(node.Operand, context);
        // TODO: If containing type is the same as operand type, we don't need to evaluate the symbol.
        var symbol = EvaluateSymbol(node.MethodSymbol.ContainingSymbol, context);
        var function = symbol.Get<FunctionValue>(node.MethodSymbol);
        var value = function.Invoke(operand);
        return value;
    }
}
