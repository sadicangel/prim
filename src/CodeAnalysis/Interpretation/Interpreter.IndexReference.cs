using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ReferenceValue EvaluateIndexReference(BoundIndexReference node, Context context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        var index = EvaluateExpression(node.Index, context);
        var array = expression as ArrayValue ?? (expression as ReferenceValue)?.ReferencedValue as ArrayValue
            ?? throw new UnreachableException($"Unexpected expression type '{expression.Type}'");

        var value = new ReferenceValue(
            node.Type,
            getReferencedValue: () => array[index],
            setReferencedValue: pv => array[index] = pv);

        return value;
    }
}
