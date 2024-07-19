using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStructDeclaration(
    SyntaxNode Syntax,
    TypeSymbol TypeSymbol,
    BoundList<BoundMemberDeclaration> Members)
    : BoundDeclaration(BoundKind.StructDeclaration, Syntax, TypeSymbol)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return TypeSymbol;
        foreach (var members in Members)
            yield return members;
    }
}
