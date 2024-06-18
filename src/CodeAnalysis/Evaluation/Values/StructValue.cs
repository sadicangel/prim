using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Types;

namespace CodeAnalysis.Evaluation.Values;

internal sealed record class StructValue(StructType Value) : PrimValue(PredefinedTypes.Type, Value)
{
    private readonly Dictionary<Symbol, PrimValue> _members = [];

    public override StructType Value { get; } = Value;

    public PrimValue GetProperty(PropertySymbol symbol) => _members[symbol];
    public void SetProperty(PropertySymbol symbol, PrimValue value) => _members[symbol] = value;

    public FunctionValue GetMethod(MethodSymbol symbol) => _members[symbol] as FunctionValue
        ?? throw new UnreachableException($"Unexpected symbol '{symbol}'");
    public void SetMethod(MethodSymbol symbol, FunctionValue value) => _members[symbol] = value;

    public FunctionValue GetOperator(OperatorSymbol symbol) => _members[symbol] as FunctionValue
        ?? throw new UnreachableException($"Unexpected symbol '{symbol}'");
    public void SetOperator(OperatorSymbol symbol, FunctionValue value) => _members[symbol] = value;

    public FunctionValue GetConversion(ConversionSymbol symbol) => _members[symbol] as FunctionValue
        ?? throw new UnreachableException($"Unexpected symbol '{symbol}'");
    public void SetConversion(ConversionSymbol symbol, FunctionValue value) => _members[symbol] = value;
}
