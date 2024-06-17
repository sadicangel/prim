using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundIdentifierNameExpression(SyntaxNode Syntax, Symbol NameSymbol)
    : BoundExpression(BoundKind.IdentifierNameExpression, Syntax, NameSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return NameSymbol;
    }
}
