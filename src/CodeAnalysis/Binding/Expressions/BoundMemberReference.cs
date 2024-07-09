using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundMemberReference(
    SyntaxNode Syntax,
    BoundExpression Expression,
    Symbol Symbol)
    : BoundReference(BoundKind.MemberReference, Syntax, Symbol)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return Symbol;
    }
}
