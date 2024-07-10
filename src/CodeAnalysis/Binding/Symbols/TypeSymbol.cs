using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class TypeSymbol(
    SyntaxNode Syntax,
    PrimType Type,
    Symbol? ContainingSymbol)
    : Symbol(
        BoundKind.TypeSymbol,
        Syntax,
        Type.Name,
        Type,
        ContainingSymbol,
        IsReadOnly: true,
        IsStatic: true)
{
    public static TypeSymbol FromType(PrimType type, Symbol? containingSymbol, SyntaxNode? syntax = null)
    {
        return new TypeSymbol(
            syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            type,
            containingSymbol);
    }
}
