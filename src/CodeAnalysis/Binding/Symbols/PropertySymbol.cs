using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(
    SyntaxNode Syntax,
    string Name,
    PrimType Type,
    Symbol? ContainingSymbol,
    bool IsReadOnly,
    bool IsStatic)
    : Symbol(
        BoundKind.PropertySymbol,
        Syntax,
        Name,
        Type,
        ContainingSymbol,
        IsReadOnly,
        IsStatic)
{
    public static PropertySymbol FromProperty(Property property, Symbol containingSymbol, SyntaxNode syntax)
    {
        return new PropertySymbol(
            syntax,
            property.Name,
            property.Type,
            containingSymbol,
            property.IsReadOnly,
            property.IsStatic);
    }
}
