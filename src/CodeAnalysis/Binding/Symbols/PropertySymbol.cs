using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(SyntaxNode Syntax, Property Property, bool IsReadOnly, bool IsStatic)
    : Symbol(BoundKind.PropertySymbol, Syntax, Property.Name, IsReadOnly, IsStatic)
{
    public override PrimType Type { get; } = Property.Type;
}
