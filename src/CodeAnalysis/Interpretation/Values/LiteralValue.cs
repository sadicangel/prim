using CodeAnalysis.Binding.Symbols;
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

    internal override FunctionValue GetMethod(MethodSymbol symbol) => Struct.GetMethod(symbol);
    internal override void SetMethod(MethodSymbol symbol, FunctionValue value) => throw new NotSupportedException();
    internal override FunctionValue GetOperator(OperatorSymbol symbol) => Struct.GetOperator(symbol);
    internal override void SetOperator(OperatorSymbol symbol, FunctionValue value) => throw new NotSupportedException();
    internal override FunctionValue GetConversion(FunctionSymbol symbol) => Struct.GetConversion(symbol);
    internal override void SetConversion(FunctionSymbol symbol, FunctionValue value) => throw new NotSupportedException();
}

