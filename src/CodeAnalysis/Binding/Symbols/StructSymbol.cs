using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class StructSymbol(SyntaxNode SyntaxNode, string Name, NamedType Type)
    : Symbol(BoundKind.Struct, SyntaxNode, Name)
{
    public override NamedType Type { get; } = Type;
}
