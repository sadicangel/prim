using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundIfExpression LowerIfExpression(BoundIfExpression node)
    {
        var condition = LowerExpression(node.Condition);
        var then = LowerExpression(node.Then);
        var @else = LowerExpression(node.Else);

        // TODO: Actually lower this to GOTO expressions.
        if (ReferenceEquals(condition, node.Condition) && ReferenceEquals(then, node.Then) && ReferenceEquals(@else, node.Else))
            return node;

        return node with
        {
            Condition = condition,
            Then = then,
            Else = @else
        };
    }
}
