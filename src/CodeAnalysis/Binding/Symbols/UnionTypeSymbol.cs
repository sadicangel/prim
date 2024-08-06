using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class UnionTypeSymbol : TypeSymbol
{
    public UnionTypeSymbol(SyntaxNode syntax, IEnumerable<TypeSymbol> types, ModuleSymbol containingModule)
        : base(
            BoundKind.UnionTypeSymbol,
            syntax,
            string.Join(" | ", types.Select(t => t.Name).Order(NaturalSortStringComparer.OrdinalIgnoreCase)),
            containingModule)
    {
        Types = [.. types];
        foreach (var unionType in Types)
        {
            AddConversion(
                SyntaxKind.ImplicitKeyword,
                new LambdaTypeSymbol([new Parameter("x", unionType)], this, containingModule));
        }
    }

    public BoundList<TypeSymbol> Types { get; }

    public override bool IsNever => Types.Any(t => t.IsNever);

    public bool Equals(UnionTypeSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();

    internal override bool IsConvertibleFrom(TypeSymbol type, out ConversionSymbol? conversion)
    {
        conversion = null;
        if (type == this || Types.Any(t => t == type))
        {
            return true;
        }

        return false;
    }
}
