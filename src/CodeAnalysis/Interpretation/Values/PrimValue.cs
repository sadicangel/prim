using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;
internal abstract record class PrimValue(TypeSymbol Type)
{
    internal Dictionary<Symbol, PrimValue> Members { get; } = [];

    public abstract object Value { get; }

    internal virtual PrimValue Get(Symbol symbol) => Members[symbol];
    internal T Get<T>(Symbol symbol) where T : PrimValue => (T)Get(symbol);
    internal virtual void Add(Symbol symbol, PrimValue value) => Members.Add(symbol, value);
    internal virtual void Set(Symbol symbol, PrimValue value) => Members[symbol] = value;

    public abstract override int GetHashCode();
    public abstract bool Equals(PrimValue? other);

    public sealed override string ToString() => $"{Value}: {Type.Name}";
}
