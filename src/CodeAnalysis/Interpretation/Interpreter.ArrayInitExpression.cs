using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ArrayValue EvaluateArrayInitExpression(BoundArrayInitExpression node, InterpreterContext context)
    {
        if (node.Type is not ArrayType arrayType)
            throw new UnreachableException($"Unexpected array type '{node.Type.Name}'");

        var elements = new PrimValue[node.Elements.Count];
        for (var i = 0; i < node.Elements.Count; ++i)
            elements[i] = EvaluateExpression(node.Elements[i], context);

        return new ArrayValue(arrayType, elements);
    }
}
