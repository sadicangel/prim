using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class StructSymbol(SyntaxNode Syntax, StructType Type)
    : Symbol(BoundKind.Struct, Syntax, Type.Name)
{
    public override StructType Type { get; } = Type;
}
