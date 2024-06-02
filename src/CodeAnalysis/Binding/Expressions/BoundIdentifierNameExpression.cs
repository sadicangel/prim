using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundIdentifierNameExpression(SyntaxNode SyntaxNode, Symbol NameSymbol)
    : BoundExpression(BoundKind.IdentifierNameExpression, SyntaxNode, NameSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return NameSymbol;
    }
}
