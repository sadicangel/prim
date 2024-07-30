using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateStackInstantiationExpression(BoundStackInstantiationExpression node, Context context)
    {
        var value = ValueFactory.Create(node.Type, node.Expression, context);
        return value;
    }
}
