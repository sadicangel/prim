using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundMemberReference(
    SyntaxNode Syntax,
    BoundExpression Expression,
    Symbol NameSymbol)
    : BoundReference(BoundKind.MemberReference, Syntax, NameSymbol, NameSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return NameSymbol;
    }
}
