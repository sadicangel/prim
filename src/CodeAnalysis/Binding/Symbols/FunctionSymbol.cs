using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class FunctionSymbol(
    SyntaxNode Syntax,
    string Name,
    FunctionType FunctionType,
    Symbol ContainingSymbol,
    NamespaceSymbol NamespaceSymbol,
    bool IsReadOnly,
    bool IsStatic,
    BoundList<VariableSymbol> Parameters)
    : Symbol(
        BoundKind.FunctionSymbol,
        Syntax,
        Name,
        FunctionType,
        ContainingSymbol,
        NamespaceSymbol,
        IsReadOnly,
        IsStatic)
{
    public PrimType ReturnType { get => FunctionType.ReturnType; }
}
