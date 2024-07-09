using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateConversionExpression(BoundConversionExpression node, InterpreterContext context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        var function = expression.Get<FunctionValue>(node.ConversionSymbol);
        var value = function.Invoke(expression);
        return value;
    }
}
