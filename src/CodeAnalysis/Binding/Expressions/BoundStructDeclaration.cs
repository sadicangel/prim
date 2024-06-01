using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStructDeclaration(
    SyntaxNode SyntaxNode,
    StructSymbol Symbol,
    BoundList<BoundPropertyDeclaration> Properties)
    : BoundDeclaration(BoundKind.StructDeclaration, SyntaxNode, Symbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Symbol;
        foreach (var property in Properties)
            yield return property;
    }
}
