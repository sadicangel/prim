using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStructDeclaration(
    SyntaxNode SyntaxNode,
    StructSymbol StructSymbol,
    BoundList<BoundPropertyDeclaration> Properties)
    : BoundDeclaration(BoundKind.StructDeclaration, SyntaxNode, StructSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return StructSymbol;
        foreach (var property in Properties)
            yield return property;
    }
}
