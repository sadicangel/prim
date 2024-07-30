using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class ConversionSymbol(
    SyntaxKind ConversionKind,
    SyntaxNode Syntax,
    string Name,
    LambdaTypeSymbol LambdaType,
    ModuleSymbol ContainingModule,
    TypeSymbol ContainingType)
    : Symbol(
        BoundKind.ConversionSymbol,
        Syntax,
        Name,
        LambdaType,
        ContainingModule,
        IsStatic: true,
        IsReadOnly: true)
{
    public VariableSymbol Parameter { get => LambdaType.Parameters[0]; }
    public TypeSymbol ReturnType { get => LambdaType.ReturnType; }
    public override IEnumerable<Symbol> DeclaredSymbols => LambdaType.Parameters;

    public bool IsImplicit { get => ConversionKind is SyntaxKind.ImplicitKeyword; }
    public bool IsExplicit { get => ConversionKind is SyntaxKind.ExplicitKeyword; }

    public bool Equals(ConversionSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
