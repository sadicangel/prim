using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class UnionTypeSymbol : TypeSymbol
{
    public UnionTypeSymbol(SyntaxNode syntax, BoundList<TypeSymbol> types)
        : base(
            BoundKind.UnionTypeSymbol,
            syntax,
            string.Join(" | ", types.Select(t => t.Name).Order(NaturalSortStringComparer.OrdinalIgnoreCase)),
            PredefinedSymbols.Type)
    {
        Types = types;
        foreach (var type in Types)
        {
            AddConversion(
                SyntaxKind.ImplicitKeyword,
                new LambdaTypeSymbol([new Parameter("x", type)], this));
        }
    }

    public BoundList<TypeSymbol> Types { get; }

    public bool Equals(UnionTypeSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();

    public override bool IsNever => Types.Any(t => t.IsNever);
}
