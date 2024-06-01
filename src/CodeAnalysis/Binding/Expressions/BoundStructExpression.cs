using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStructExpression(
     SyntaxNode SyntaxNode,
     PrimType Type,
     BoundList<BoundPropertyExpression> Properties)
    : BoundExpression(BoundKind.StructExpression, SyntaxNode, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var property in Properties)
            yield return property;
    }
}
