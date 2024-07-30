using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateConversionExpression(BoundConversionExpression node, Context context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        var containingValue = node.ConversionSymbol.ContainingSymbol == expression.Type
            ? expression
            : ValueFactory.Create((TypeSymbol)node.ConversionSymbol.ContainingSymbol, node.Expression, context);
        var conversion = containingValue.Get<LambdaValue>(node.ConversionSymbol);
        var value = conversion.Invoke(expression);
        return value;
    }
}
