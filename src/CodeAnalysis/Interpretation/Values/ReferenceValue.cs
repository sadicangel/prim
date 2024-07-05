using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class ReferenceValue : PrimValue
{
    private readonly Func<PrimValue> _getReferencedValue;
    private readonly Action<PrimValue> _setReferencedValue;

    public ReferenceValue(
        PrimType type,
        Func<PrimValue> getReferencedValue,
        Action<PrimValue> setReferencedValue) : base(type)
    {
        _getReferencedValue = getReferencedValue;
        _setReferencedValue = setReferencedValue;
    }

    public PrimValue ReferencedValue { get => _getReferencedValue(); set => _setReferencedValue(value); }

    public override object Value => ReferencedValue.Value;

    internal override PrimValue GetMember(Symbol symbol) => ReferencedValue.GetMember(symbol);
    internal override void SetMember(Symbol symbol, PrimValue value) => ReferencedValue.SetMember(symbol, value);

    internal override PrimValue GetProperty(PropertySymbol symbol) => ReferencedValue.GetProperty(symbol);
    internal override void SetProperty(PropertySymbol symbol, PrimValue value) => ReferencedValue.SetProperty(symbol, value);

    internal override FunctionValue GetMethod(MethodSymbol symbol) => ReferencedValue.GetMethod(symbol);
    internal override void SetMethod(MethodSymbol symbol, FunctionValue value) => ReferencedValue.SetMethod(symbol, value);

    internal override FunctionValue GetOperator(OperatorSymbol symbol) => ReferencedValue.GetOperator(symbol);
    internal override void SetOperator(OperatorSymbol symbol, FunctionValue value) => ReferencedValue.SetOperator(symbol, value);

    internal override FunctionValue GetConversion(FunctionSymbol symbol) => ReferencedValue.GetConversion(symbol);
    internal override void SetConversion(FunctionSymbol symbol, FunctionValue value) => ReferencedValue.SetConversion(symbol, value);
}
