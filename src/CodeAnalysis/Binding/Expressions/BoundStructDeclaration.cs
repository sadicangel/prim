using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStructDeclaration(
    SyntaxNode Syntax,
    StructTypeSymbol StructTypeSymbol,
    BoundList<BoundMemberDeclaration> Members)
    : BoundDeclaration(BoundKind.StructDeclaration, Syntax, StructTypeSymbol)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return StructTypeSymbol;
        foreach (var members in Members)
            yield return members;
    }
}
