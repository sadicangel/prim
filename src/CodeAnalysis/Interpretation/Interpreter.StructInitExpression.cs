using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ObjectValue EvaluateStructInitExpression(BoundStructInitExpression node, InterpreterContext context)
    {
        var structValue = context.EvaluatedScope.Lookup(node.TypeSymbol) as StructValue
            ?? throw new UnreachableException($"Unexpected struct value '{context.EvaluatedScope.Lookup(node.TypeSymbol)}'");

        var objectValue = new ObjectValue(structValue);

        foreach (var property in node.Properties)
        {
            var propertyValue = EvaluateExpression(property.Expression, context);
            objectValue[property.PropertySymbol] = propertyValue;
        }
        return objectValue;
    }
}
