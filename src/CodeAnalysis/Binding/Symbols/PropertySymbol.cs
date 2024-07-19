using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(
    SyntaxNode Syntax,
    string Name,
    PrimType Type,
    bool IsReadOnly,
    bool IsStatic)
    : Symbol(
        BoundKind.PropertySymbol,
        Syntax,
        Name,
        Type,
        IsReadOnly,
        IsStatic)
{
    public static PropertySymbol FromProperty(Property property, SyntaxNode syntax)
    {
        return new PropertySymbol(
            syntax,
            property.Name,
            property.Type,
            property.IsReadOnly,
            property.IsStatic);
    }
}
