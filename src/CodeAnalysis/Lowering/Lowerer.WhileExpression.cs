using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundWhileExpression LowerWhileExpression(BoundWhileExpression node)
    {
        var condition = LowerExpression(node.Condition);
        var body = LowerExpression(node.Body);
        if (ReferenceEquals(condition, node.Condition) && ReferenceEquals(body, node.Body))
            return node;

        return node with { Condition = condition, Body = body };
    }
}
