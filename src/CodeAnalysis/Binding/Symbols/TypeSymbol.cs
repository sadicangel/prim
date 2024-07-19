using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class TypeSymbol(
    SyntaxNode Syntax,
    PrimType Type)
    : Symbol(
        BoundKind.TypeSymbol,
        Syntax,
        Type.Name,
        Type,
        IsReadOnly: true,
        IsStatic: true)
{
    public static TypeSymbol FromType(PrimType type, SyntaxNode? syntax = null)
    {
        return new TypeSymbol(
            syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            type);
    }
}
