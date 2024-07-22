
using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

internal sealed record class UnionValue : PrimValue
{
    public UnionValue(UnionTypeSymbol unionType, PrimValue value) : base(unionType)
    {
        Value = value;

        foreach (var type in unionType.Types)
        {
            var conversion = unionType.GetConversion(type, unionType)
                ?? throw new UnreachableException($"Missing conversion from {type} to {unionType}");
            Set(
                conversion,
                new LambdaValue(conversion.LambdaType, (PrimValue x) => new UnionValue(unionType, x)));
        }
    }

    public override PrimValue Value { get; }

    public bool Equals(UnionValue? other) => Type == other?.Type && Value.Equals(other.Value);
    public override int GetHashCode() => HashCode.Combine(Type, Value);
}
