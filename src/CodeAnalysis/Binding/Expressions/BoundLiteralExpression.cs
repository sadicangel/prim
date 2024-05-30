using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLiteralExpression(BoundKind BoundKind, SyntaxNode SyntaxNode, object? Value)
    : BoundExpression(BoundKind, SyntaxNode)
{
    public override IEnumerable<BoundNode> Children() => [];
}
