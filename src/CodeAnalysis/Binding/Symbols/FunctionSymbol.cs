using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class FunctionSymbol(
    SyntaxNode Syntax,
    string Name,
    FunctionType Type,
    BoundList<VariableSymbol> Parameters,
    bool IsReadOnly)
    : Symbol(BoundKind.FunctionSymbol, Syntax, Name, IsReadOnly)
{
    public override FunctionType Type { get; } = Type;
    public PrimType ReturnType { get => Type.ReturnType; }
}
