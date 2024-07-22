using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ReferenceValue EvaluatePropertyReference(BoundPropertyReference node, InterpreterContext context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        var propertyReference = new ReferenceValue(
            node.Symbol.Type,
            () => expression.Get(node.Symbol),
            pv => expression.Set(node.Symbol, pv));
        return propertyReference;
    }
}
