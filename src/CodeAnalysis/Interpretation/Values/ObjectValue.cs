using System.Collections;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class ObjectValue
    : PrimValue, IEnumerable<KeyValuePair<PropertySymbol, PrimValue>>
{
    public ObjectValue(StructValue @struct) : base(@struct.StructType)
    {
        Struct = @struct;
        foreach (var (symbol, value) in Struct.Members)
        {
            if (symbol is PropertySymbol propertySymbol)
                SetProperty(propertySymbol, value);
        }
    }

    public StructValue Struct { get; }

    public PrimValue this[PropertySymbol symbol] { get => GetProperty(symbol); set => SetProperty(symbol, value); }

    public override object Value => Members;

    public int Count { get => Members.Count; }

    internal override FunctionValue GetMethod(MethodSymbol symbol) => Struct.GetMethod(symbol);
    internal override void SetMethod(MethodSymbol symbol, FunctionValue value) => throw new NotSupportedException();
    internal override FunctionValue GetOperator(OperatorSymbol symbol) => Struct.GetOperator(symbol);
    internal override void SetOperator(OperatorSymbol symbol, FunctionValue value) => throw new NotSupportedException();
    internal override FunctionValue GetConversion(ConversionSymbol symbol) => Struct.GetConversion(symbol);
    internal override void SetConversion(ConversionSymbol symbol, FunctionValue value) => throw new NotSupportedException();

    public IEnumerator<KeyValuePair<PropertySymbol, PrimValue>> GetEnumerator()
    {
        foreach (var (symbol, value) in Members)
            yield return new KeyValuePair<PropertySymbol, PrimValue>((PropertySymbol)symbol, value);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
