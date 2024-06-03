using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStructDeclaration(
    SyntaxNode SyntaxNode,
    StructSymbol StructSymbol,
    BoundList<BoundMemberDeclaration> Members)
    : BoundDeclaration(BoundKind.StructDeclaration, SyntaxNode, StructSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return StructSymbol;
        foreach (var members in Members)
            yield return members;
    }
}
