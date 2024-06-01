using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class VariableSymbol(SyntaxNode SyntaxNode, string Name, PrimType Type, bool IsReadOnly)
    : Symbol(BoundKind.Variable, SyntaxNode, Name)
{
    public override PrimType Type { get; } = Type;
}
