using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class TypeSymbol(
    SyntaxNode Syntax,
    PrimType Type,
    Symbol ContainingSymbol,
    NamespaceSymbol NamespaceSymbol)
    : Symbol(
        BoundKind.TypeSymbol,
        Syntax,
        Type.Name,
        Type,
        ContainingSymbol,
        NamespaceSymbol,
        IsReadOnly: true,
        IsStatic: true)
{
    public static TypeSymbol FromType(PrimType type, Symbol containingSymbol, SyntaxNode? syntax = null)
    {
        return new TypeSymbol(
            syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            type,
            containingSymbol,
            containingSymbol.ContainingNamespace);
    }
}
