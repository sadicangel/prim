using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(SyntaxNode SyntaxNode, string Name, PrimType Type, bool IsReadOnly)
    : Symbol(BoundKind.Property, SyntaxNode, Name)
{
    public override PrimType Type { get; } = Type;
}
