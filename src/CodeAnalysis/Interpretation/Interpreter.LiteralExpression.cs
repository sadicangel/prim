using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static LiteralValue EvaluateLiteralExpression(BoundLiteralExpression node, Context context)
    {
        _ = context;
        var scope = GlobalEvaluatedScope.Instance;
        var structValue = (StructValue)scope.Lookup(node.Type);
        var literalValue = node.Value;
        Debug.Assert(literalValue.GetType() == node.Type.GetCrlType());
        return new LiteralValue(structValue, literalValue);
    }
}
