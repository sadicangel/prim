using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(
    SyntaxNode Syntax,
    Property Property)
    : Symbol(BoundKind.Property, Syntax, $"{Property.Name}")
{
    public override PrimType Type { get; } = Property.Type;
}
