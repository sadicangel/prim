using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateUnaryExpression(BoundUnaryExpression node, Context context)
    {
        var operand = EvaluateExpression(node.Operand, context);
        var symbol = node.MethodSymbol.ContainingSymbol == operand.Type
            ? operand
            : ValueFactory.Create((TypeSymbol)node.MethodSymbol.ContainingSymbol, node.Operand, context);
        var function = symbol.Get<LambdaValue>(node.MethodSymbol);
        var value = function.Invoke(operand);
        return value;
    }
}
