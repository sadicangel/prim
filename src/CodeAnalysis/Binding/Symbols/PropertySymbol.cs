using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(SyntaxNode Syntax, Property Property, StructSymbol? ContainingSymbol = null)
    : MemberSymbol(BoundKind.Property, Syntax, Property.Name, ContainingSymbol)
{
    public override PrimType Type { get; } = Property.Type;
}
