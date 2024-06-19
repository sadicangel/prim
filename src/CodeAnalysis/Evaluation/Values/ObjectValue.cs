using System.Collections;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Evaluation.Values;
internal sealed record class ObjectValue(StructValue Struct)
    : PrimValue(Struct.StructType), IEnumerable<KeyValuePair<PropertySymbol, PrimValue>>
{
    private readonly Dictionary<PropertySymbol, PrimValue> _properties = Struct.Members
        .Where(e => e.Key is PropertySymbol)
        .ToDictionary(e => (PropertySymbol)e.Key, e => e.Value);

    public PrimValue this[PropertySymbol symbol] { get => _properties[symbol]; set => _properties[symbol] = value; }

    public override object Value => _properties;

    public int Count { get => _properties.Count; }

    public IEnumerator<KeyValuePair<PropertySymbol, PrimValue>> GetEnumerator() => _properties.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
