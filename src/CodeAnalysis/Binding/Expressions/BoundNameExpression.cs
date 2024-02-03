using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundNameExpression(
    SyntaxNode SyntaxNode,
    Symbol Symbol
)
    : BoundExpression(BoundNodeKind.NameExpression, SyntaxNode, Symbol.Type)
{
    public override IEnumerable<BoundNode> Children() => Enumerable.Empty<BoundNode>();
}
