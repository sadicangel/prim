using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class LiteralValue : PrimValue
{
    public LiteralValue(StructValue @struct, TypeSymbol type, object value) : base(type)
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

    internal override PrimValue Get(Symbol symbol) => symbol.IsStatic ? Struct.Get(symbol) : base.Get(symbol);
}

