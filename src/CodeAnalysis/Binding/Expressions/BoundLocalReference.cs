using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLocalReference(SyntaxNode Syntax, Symbol Symbol, TypeSymbol Type)
    : BoundReference(BoundKind.LocalReference, Syntax, Symbol, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Symbol;
        yield return Type;
    }
}
