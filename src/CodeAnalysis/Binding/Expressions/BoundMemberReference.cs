using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundMemberReference(
    SyntaxNode Syntax,
    BoundExpression Expression,
    Symbol Symbol,
    TypeSymbol Type)
    : BoundReference(BoundKind.MemberReference, Syntax, Symbol, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return Symbol;
        yield return Type;
    }
}
