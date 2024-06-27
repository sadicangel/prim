using System.Diagnostics;
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

    internal virtual PrimValue GetMember(Symbol symbol) => Members[symbol];
    internal virtual void SetMember(Symbol symbol, PrimValue value) => Members[symbol] = value;

    internal virtual PrimValue GetProperty(PropertySymbol symbol) => Members[symbol];
    internal virtual void SetProperty(PropertySymbol symbol, PrimValue value) => Members[symbol] = value;

    internal virtual FunctionValue GetMethod(MethodSymbol symbol) => Members[symbol] as FunctionValue
        ?? throw new UnreachableException($"Unexpected symbol '{symbol}'");
    internal virtual void SetMethod(MethodSymbol symbol, FunctionValue value) => Members[symbol] = value;

    internal virtual FunctionValue GetOperator(OperatorSymbol symbol) => Members[symbol] as FunctionValue
        ?? throw new UnreachableException($"Unexpected symbol '{symbol}'");
    internal virtual void SetOperator(OperatorSymbol symbol, FunctionValue value) => Members[symbol] = value;

    internal virtual FunctionValue GetConversion(ConversionSymbol symbol) => Members[symbol] as FunctionValue
        ?? throw new UnreachableException($"Unexpected symbol '{symbol}'");
    internal virtual void SetConversion(ConversionSymbol symbol, FunctionValue value) => Members[symbol] = value;
}
