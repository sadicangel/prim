using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateUnaryExpression(BoundUnaryExpression node, InterpreterContext context)
    {
        var operand = EvaluateExpression(node.Operand, context);
        // TODO: This can't be done on the expression type.
        //var symbol = EvaluateSymbol(node.MethodSymbol.ContainingSymbol, context);
        var function = operand.Get<FunctionValue>(node.MethodSymbol);
        var value = function.Invoke(operand);
        return value;
    }
}
