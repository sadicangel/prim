using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(
    SyntaxNode Syntax,
    Property Property)
    : Symbol(BoundKind.Property, Syntax, $"{Property.Name}")
{
    public override PrimType Type { get; } = Property.Type;
}
