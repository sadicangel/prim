using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ArrayValue EvaluateArrayInitExpression(BoundArrayInitExpression node, Context context)
    {
        if (node.Type is not ArrayTypeSymbol arrayType)
            throw new UnreachableException($"Unexpected array type '{node.Type.Name}'");

        var elements = new PrimValue[node.Elements.Count];
        for (var i = 0; i < node.Elements.Count; ++i)
            elements[i] = EvaluateExpression(node.Elements[i], context);

        return new ArrayValue(arrayType, elements);
    }
}
