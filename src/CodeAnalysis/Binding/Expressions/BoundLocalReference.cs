using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLocalReference(SyntaxNode Syntax, Symbol Symbol)
    : BoundReference(BoundKind.LocalReference, Syntax, Symbol)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Symbol;
    }
}
