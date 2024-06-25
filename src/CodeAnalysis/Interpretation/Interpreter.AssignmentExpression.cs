using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateAssignmentExpression(BoundAssignmentExpression node, InterpreterContext context)
    {
        var left = EvaluateExpression(node.Left, context);
        if (left is not ReferenceValue @ref)
        {
            throw new NotSupportedException($"Unexpected left hand side of {nameof(BoundAssignmentExpression)} '{node.Left.BoundKind}'");
        }

        var value = EvaluateExpression(node.Right, context);
        @ref.ReferencedValue = value;
        return value;
    }
}
