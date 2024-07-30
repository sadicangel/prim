using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class ConversionSymbol(
    SyntaxKind ConversionKind,
    SyntaxNode Syntax,
    string Name,
    LambdaTypeSymbol LambdaType,
    Symbol ContainingSymbol)
    : Symbol(
        BoundKind.ConversionSymbol,
        Syntax,
        Name,
        LambdaType,
        IsStatic: true,
        IsReadOnly: true)
{
    public VariableSymbol Parameter { get => LambdaType.Parameters[0]; }
    public TypeSymbol ReturnType { get => LambdaType.ReturnType; }

    public bool IsImplicit { get => ConversionKind is SyntaxKind.ImplicitKeyword; }
    public bool IsExplicit { get => ConversionKind is SyntaxKind.ExplicitKeyword; }

    public bool Equals(ConversionSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
