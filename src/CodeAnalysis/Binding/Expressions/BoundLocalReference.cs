using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLocalReference(SyntaxNode Syntax, Symbol NameSymbol)
    : BoundReference(BoundKind.LocalReference, Syntax, NameSymbol, NameSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return NameSymbol;
    }
}
