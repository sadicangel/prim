using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateAssignmentExpression(BoundAssignmentExpression node, Context context)
    {
        var left = EvaluateExpression(node.Left, context);
        if (left is not ReferenceValue @ref)
        {
            throw new NotSupportedException($"Unexpected left hand side of {nameof(BoundAssignmentExpression)} '{node.Left.BoundKind}'");
        }

        var right = ValueFactory.Create(node.Type, node.Right, context);
        @ref.ReferencedValue = right;
        return right;
    }
}
