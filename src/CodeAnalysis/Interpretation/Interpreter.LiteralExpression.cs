using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static InstanceValue EvaluateLiteralExpression(BoundLiteralExpression node, Context context)
    {
        _ = context;
        var structValue = (StructValue)context.EvaluatedScope.LookupLocal(node.Type);
        var literalValue = node.Value;
        Debug.Assert(literalValue.GetType() == node.Type.GetCrlType());
        return new InstanceValue(structValue, literalValue);
    }
}
