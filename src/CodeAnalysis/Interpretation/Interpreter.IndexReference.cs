using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ReferenceValue EvaluateIndexReference(BoundIndexReference node, InterpreterContext context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        // TODO: Index must be something else, or we need to use 2 operators?
        if (expression is not ArrayValue array)
            throw new NotImplementedException(expression.GetType().Name);

        var index = EvaluateExpression(node.Index, context);

        var value = new ReferenceValue(
            node.Type,
            getReferencedValue: () => array[index],
            setReferencedValue: pv => array[index] = pv);

        //var function = expression.GetOperator(node.OperatorSymbol);
        //var value = function.Invoke(index);
        return value;
    }
}
