using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundGlobalReference(SyntaxNode Syntax, Symbol Symbol, TypeSymbol Type)
    : BoundReference(BoundKind.GlobalReference, Syntax, Symbol, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Symbol;
        yield return Type;
    }
}
