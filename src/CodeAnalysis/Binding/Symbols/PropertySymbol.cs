using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(SyntaxNode SyntaxNode, Property Property)
    : Symbol(BoundKind.Property, SyntaxNode, Property.Name)
{
    public override PrimType Type { get; } = Property.Type;

    public bool IsReadOnly { get => Property.IsReadOnly; }
}
