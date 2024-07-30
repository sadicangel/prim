using System.Runtime.CompilerServices;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class ReferenceValue : PrimValue
{
    private readonly Func<PrimValue> _getReferencedValue;
    private readonly Action<PrimValue> _setReferencedValue;

    public ReferenceValue(
        TypeSymbol type,
        Func<PrimValue> getReferencedValue,
        Action<PrimValue> setReferencedValue) : base(type)
    {
        _getReferencedValue = getReferencedValue;
        _setReferencedValue = setReferencedValue;
    }

    public PrimValue ReferencedValue { get => _getReferencedValue(); set => _setReferencedValue(value); }

    public override object Value => ReferencedValue.Value;

    public bool Equals(ReferenceValue? other) => ReferenceEquals(this, other);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    internal override PrimValue Get(Symbol symbol) => ReferencedValue.Get(symbol);
    internal override void Add(Symbol symbol, PrimValue value) => ReferencedValue.Add(symbol, value);
}
