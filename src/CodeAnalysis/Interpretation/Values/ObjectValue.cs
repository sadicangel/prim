using System.Collections;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class ObjectValue
    : PrimValue, IEnumerable<KeyValuePair<PropertySymbol, PrimValue>>
{
    public ObjectValue(StructValue @struct) : base(@struct.StructType)
    {
        Struct = @struct;
        foreach (var (memberSymbol, memberValue) in Struct.Members)
        {
            if (!memberSymbol.IsStatic)
                Set(memberSymbol, memberValue);
        }
    }

    public StructValue Struct { get; }

    public PrimValue this[PropertySymbol symbol] { get => Get(symbol); set => Set(symbol, value); }

    public override object Value => Members;

    public int Count { get => Members.Count; }

    public bool Equals(ObjectValue? other) => Struct == other?.Struct && Members.SequenceEqual(other.Members);
    public override int GetHashCode() => HashCode.Combine(Struct, Members);

    internal override PrimValue Get(Symbol symbol) => symbol.IsStatic ? Struct.Get(symbol) : base.Get(symbol);

    public IEnumerator<KeyValuePair<PropertySymbol, PrimValue>> GetEnumerator()
    {
        foreach (var (symbol, value) in Members)
            yield return new KeyValuePair<PropertySymbol, PrimValue>((PropertySymbol)symbol, value);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
