using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class VariableSymbol(SyntaxNode Syntax, string Name, PrimType Type, bool IsReadOnly)
    : Symbol(BoundKind.Variable, Syntax, Name)
{
    public override PrimType Type { get; } = Type;
}
