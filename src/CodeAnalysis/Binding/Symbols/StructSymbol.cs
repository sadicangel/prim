using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class StructSymbol(SyntaxNode Syntax, StructType Type)
    : Symbol(BoundKind.StructSymbol, Syntax, Type.Name, IsReadOnly: true)
{
    public override StructType Type { get; } = Type;
}
