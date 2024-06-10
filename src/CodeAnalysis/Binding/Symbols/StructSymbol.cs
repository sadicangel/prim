using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class StructSymbol(SyntaxNode SyntaxNode, StructType Type)
    : Symbol(BoundKind.Struct, SyntaxNode, Type.Name)
{
    public override StructType Type { get; } = Type;
}
