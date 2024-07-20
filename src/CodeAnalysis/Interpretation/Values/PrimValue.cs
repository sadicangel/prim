using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;
internal abstract record class PrimValue(TypeSymbol Type)
{
    internal static LiteralValue Unit { get; } = new LiteralValue(GlobalEvaluatedScope.Instance.Unit, PredefinedSymbols.Unit, CodeAnalysis.Unit.Value);
    internal static LiteralValue True { get; } = new LiteralValue(GlobalEvaluatedScope.Instance.Bool, PredefinedSymbols.Bool, true);
    internal static LiteralValue False { get; } = new LiteralValue(GlobalEvaluatedScope.Instance.Bool, PredefinedSymbols.Bool, false);

    internal Dictionary<Symbol, PrimValue> Members { get; } = [];

    public abstract object Value { get; }

    internal virtual PrimValue Get(Symbol symbol) => Members[symbol];
    internal T Get<T>(Symbol symbol) where T : PrimValue => (T)Get(symbol);
    internal virtual void Set(Symbol symbol, PrimValue value) => Members[symbol] = value;
}
