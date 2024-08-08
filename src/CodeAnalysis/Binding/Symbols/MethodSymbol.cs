using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class MethodSymbol(
    SyntaxNode Syntax,
    string NameNoMangling,
    LambdaTypeSymbol LambdaType,
    TypeSymbol ContainingType,
    bool IsStatic,
    bool IsReadOnly)
    : Symbol(
        BoundKind.MethodSymbol,
        Syntax,
        $"{NameNoMangling}<{string.Join(',', LambdaType.Parameters.Select(p => p.Type.Name))}>",
        LambdaType,
        ContainingType.ContainingModule,
        ContainingType.ContainingModule,
        IsStatic,
        IsReadOnly)
{
    public BoundList<VariableSymbol> Parameters { get => LambdaType.Parameters; }
    public TypeSymbol ReturnType { get => LambdaType.ReturnType; }
    public override IEnumerable<Symbol> DeclaredSymbols => Parameters;

    public bool Equals(MethodSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
