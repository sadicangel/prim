using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class FunctionSymbol(
    SyntaxNode Syntax,
    string Name,
    LambdaTypeSymbol LambdaType,
    bool IsStatic,
    bool IsReadOnly)
    : Symbol(
        BoundKind.FunctionSymbol,
        Syntax,
        Name,
        LambdaType,
        IsStatic,
        IsReadOnly)
{
    public BoundList<VariableSymbol> Parameters { get => LambdaType.Parameters; }
    public TypeSymbol ReturnType { get => LambdaType.ReturnType; }
}
