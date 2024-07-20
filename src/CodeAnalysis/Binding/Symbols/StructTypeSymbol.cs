using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class StructTypeSymbol : TypeSymbol
{
    public StructTypeSymbol(SyntaxNode syntax, string name)
        : base(BoundKind.StructTypeSymbol, syntax, name, RuntimeType)
    {
    }

    private StructTypeSymbol()
        : base(BoundKind.StructTypeSymbol, SyntaxFactory.SyntheticToken(SyntaxKind.TypeKeyword), PredefinedSymbolNames.Type, null!)
    {
        Type = this;
    }

    public static StructTypeSymbol RuntimeType { get; } = new StructTypeSymbol();

    public override bool IsNever => Name == PredefinedSymbolNames.Never;
}
