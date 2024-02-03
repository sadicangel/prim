using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLiteralExpression(
    SyntaxNode SyntaxNode,
    PrimType Type,
    object? Value
)
    : BoundExpression(BoundNodeKind.LiteralExpression, SyntaxNode, Type)
{
    public override IEnumerable<BoundNode> Children() => Enumerable.Empty<BoundNode>();
}
