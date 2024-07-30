using System.Collections;
using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class InstanceValue
    : PrimValue, IEnumerable<KeyValuePair<PropertySymbol, PrimValue>>
{
    private readonly object? _literalValue;

    public InstanceValue(StructValue @struct) : this(@struct, literalValue: null!) { }

    public InstanceValue(StructValue @struct, object literalValue) : base(@struct.StructType)
    {
        Struct = @struct;
        _literalValue = literalValue;
        foreach (var (memberSymbol, memberValue) in Struct.Members)
        {
            if (!memberSymbol.IsStatic)
                Add(memberSymbol, memberValue);
        }
    }

    public StructValue Struct { get; }

    public PrimValue this[PropertySymbol symbol] { get => Get(symbol); set => Set(symbol, value); }

    public override object Value => _literalValue ?? Members;

    [MemberNotNullWhen(true, nameof(_literalValue))]
    public bool IsLiteral => _literalValue is not null;

    public int Count { get => Members.Count; }

    public bool Equals(InstanceValue? other)
    {
        if (Struct != other?.Struct) return false;

        return IsLiteral ? _literalValue.Equals(other._literalValue) : Members.SequenceEqual(other.Members);
    }

    public override int GetHashCode()
    {
        if (IsLiteral)
            return HashCode.Combine(Struct, _literalValue);

        var hash = new HashCode();
        hash.Add(Struct);
        foreach (var member in Members)
            hash.Add(member);
        return hash.ToHashCode();
    }

    internal override PrimValue Get(Symbol symbol) => symbol.IsStatic ? Struct.Get(symbol) : base.Get(symbol);

    public IEnumerator<KeyValuePair<PropertySymbol, PrimValue>> GetEnumerator()
    {
        foreach (var (symbol, value) in Members)
            yield return new KeyValuePair<PropertySymbol, PrimValue>((PropertySymbol)symbol, value);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
