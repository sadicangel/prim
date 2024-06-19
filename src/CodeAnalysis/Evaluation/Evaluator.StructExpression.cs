using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Evaluation.Values;

namespace CodeAnalysis.Evaluation;
partial class Evaluator
{
    private static ObjectValue EvaluateStructExpression(BoundStructExpression node, EvaluatorContext context)
    {
        var structValue = context.EvaluatedScope.Lookup(node.StructSymbol) as StructValue
            ?? throw new UnreachableException($"Unexpected struct value '{context.EvaluatedScope.Lookup(node.StructSymbol)}'");

        var objectValue = new ObjectValue(structValue);

        foreach (var property in node.Properties)
        {
            var propertyValue = EvaluateExpression(property.Expression, context);
            objectValue[property.PropertySymbol] = propertyValue;
        }
        return objectValue;
    }
}
