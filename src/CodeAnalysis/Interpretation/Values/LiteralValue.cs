using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class LiteralValue : PrimValue
{
    public LiteralValue(StructValue @struct, object value) : base(@struct.StructType)
    {
        Struct = @struct;
        Value = value;
        foreach (var (memberSymbol, memberValue) in Struct.Members)
        {
            if (!memberSymbol.IsStatic)
                Set(memberSymbol, memberValue);
        }
    }

    public StructValue Struct { get; }
    public override object Value { get; }

    public bool Equals(LiteralValue? other) => Struct == other?.Struct && Value.Equals(other.Value);
    public override int GetHashCode() => HashCode.Combine(Struct, Value);

    internal override PrimValue Get(Symbol symbol) => symbol.IsStatic ? Struct.Get(symbol) : base.Get(symbol);
}

