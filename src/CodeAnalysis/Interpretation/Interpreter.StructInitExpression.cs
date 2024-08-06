using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static InstanceValue EvaluateStructInitExpression(BoundStructInitExpression node, Context context)
    {
        var structValue = context.EvaluatedScope.LookupGlobal(node.TypeSymbol) as StructValue
            ?? throw new UnreachableException($"Unexpected struct value '{context.EvaluatedScope.LookupLocal(node.TypeSymbol)}'");

        var objectValue = new InstanceValue(structValue);

        foreach (var property in node.Properties)
        {
            var propertyValue = EvaluateExpression(property.Expression, context);
            objectValue[property.PropertySymbol] = propertyValue;
        }
        return objectValue;
    }
}
