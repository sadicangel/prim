using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class LiteralValue : PrimValue
{
    public LiteralValue(StructValue @struct, PrimType type, object value) : base(type)
    {
        Struct = @struct;
        Value = value;
        foreach (var (symbol, value1) in Struct.Members)
        {
            if (symbol is PropertySymbol propertySymbol)
                SetProperty(propertySymbol, value1);
        }
    }

    public StructValue Struct { get; }
    public override object Value { get; }

    public static LiteralValue Unit { get; } = new LiteralValue(GlobalEvaluatedScope.Instance.Unit, PredefinedTypes.Unit, new Unit());
    public static LiteralValue True { get; } = new LiteralValue(GlobalEvaluatedScope.Instance.Bool, PredefinedTypes.Bool, true);
    public static LiteralValue False { get; } = new LiteralValue(GlobalEvaluatedScope.Instance.Bool, PredefinedTypes.Bool, false);

    internal override FunctionValue GetMethod(MethodSymbol symbol) => Struct.GetMethod(symbol);
    internal override void SetMethod(MethodSymbol symbol, FunctionValue value) => throw new NotSupportedException();
    internal override FunctionValue GetOperator(OperatorSymbol symbol) => Struct.GetOperator(symbol);
    internal override void SetOperator(OperatorSymbol symbol, FunctionValue value) => throw new NotSupportedException();
    internal override FunctionValue GetConversion(ConversionSymbol symbol) => Struct.GetConversion(symbol);
    internal override void SetConversion(ConversionSymbol symbol, FunctionValue value) => throw new NotSupportedException();
}

file sealed class Unit
{
    public override string ToString() => SyntaxFacts.GetText(SyntaxKind.NullKeyword)
        ?? throw new UnreachableException("Missing ToString method");
}

