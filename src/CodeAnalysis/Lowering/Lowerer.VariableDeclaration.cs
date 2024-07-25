using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundVariableDeclaration LowerVariableDeclaration(BoundVariableDeclaration node)
    {
        var expression = LowerExpression(node.Expression);
        if (ReferenceEquals(expression, node.Expression))
            return node;

        return node with { Expression = expression };
    }
}
