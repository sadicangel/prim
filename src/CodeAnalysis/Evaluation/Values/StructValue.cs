using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Types;

namespace CodeAnalysis.Evaluation.Values;

internal sealed record class StructValue(StructType StructType) : PrimValue(PredefinedTypes.Type)
{
    public Dictionary<Symbol, PrimValue> Members { get; } = [];

    public override StructType Value => StructType;

    public PrimValue GetProperty(PropertySymbol symbol) => Members[symbol];
    public void SetProperty(PropertySymbol symbol, PrimValue value) => Members[symbol] = value;

    public FunctionValue GetMethod(MethodSymbol symbol) => Members[symbol] as FunctionValue
        ?? throw new UnreachableException($"Unexpected symbol '{symbol}'");
    public void SetMethod(MethodSymbol symbol, FunctionValue value) => Members[symbol] = value;

    public FunctionValue GetOperator(OperatorSymbol symbol) => Members[symbol] as FunctionValue
        ?? throw new UnreachableException($"Unexpected symbol '{symbol}'");
    public void SetOperator(OperatorSymbol symbol, FunctionValue value) => Members[symbol] = value;

    public FunctionValue GetConversion(ConversionSymbol symbol) => Members[symbol] as FunctionValue
        ?? throw new UnreachableException($"Unexpected symbol '{symbol}'");
    public void SetConversion(ConversionSymbol symbol, FunctionValue value) => Members[symbol] = value;
}
