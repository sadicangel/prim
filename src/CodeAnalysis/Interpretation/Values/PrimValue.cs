using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;
public abstract record class PrimValue(PrimType Type)
{
    internal static LiteralValue Unit { get; } = new LiteralValue(GlobalEvaluatedScope.Instance.Unit, PredefinedTypes.Unit, CodeAnalysis.Unit.Value);
    internal static LiteralValue True { get; } = new LiteralValue(GlobalEvaluatedScope.Instance.Bool, PredefinedTypes.Bool, true);
    internal static LiteralValue False { get; } = new LiteralValue(GlobalEvaluatedScope.Instance.Bool, PredefinedTypes.Bool, false);

    internal Dictionary<Symbol, PrimValue> Members { get; } = [];

    public abstract object Value { get; }

    internal virtual PrimValue Get(Symbol symbol) => Members[symbol];
    internal T Get<T>(Symbol symbol) where T : PrimValue => (T)Get(symbol);
    internal virtual void Set(Symbol symbol, PrimValue value) => Members[symbol] = value;
}
