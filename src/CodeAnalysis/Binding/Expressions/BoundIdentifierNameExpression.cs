using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundIdentifierNameExpression(SyntaxNode SyntaxNode, Symbol Symbol)
    : BoundExpression(BoundKind.IdentifierNameExpression, SyntaxNode)
{
    public override IEnumerable<BoundNode> Children() => [];
}
