using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateUnaryExpression(BoundUnaryExpression node, InterpreterContext context)
    {
        var operand = EvaluateExpression(node.Operand, context);
        var function = operand.Get<FunctionValue>(node.FunctionSymbol);
        return function.Invoke(operand);
    }
}
