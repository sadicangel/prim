using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class OperatorSymbol(
    SyntaxKind OperatorKind,
    SyntaxNode Syntax,
    LambdaTypeSymbol LambdaType,
    TypeSymbol ContainingType)
    : Symbol(
        BoundKind.OperatorSymbol,
        Syntax,
        $"{SyntaxFacts.GetText(OperatorKind)}<{string.Join(',', LambdaType.Parameters.Select(p => p.Type.Name))}>",
        LambdaType,
        ContainingType.ContainingModule,
        ContainingType.ContainingModule,
        IsStatic: true,
        IsReadOnly: true)
{
    public BoundList<VariableSymbol> Parameters { get => LambdaType.Parameters; }
    public TypeSymbol ReturnType { get => LambdaType.ReturnType; }
    public override IEnumerable<Symbol> DeclaredSymbols => LambdaType.Parameters;

    public bool IsUnaryOperator { get => Parameters.Count == 1; }
    public bool IsBinaryOperator { get => Parameters.Count == 2; }

    public bool Equals(OperatorSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
