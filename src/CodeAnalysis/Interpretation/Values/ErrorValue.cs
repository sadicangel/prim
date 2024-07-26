using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class ErrorValue : PrimValue
{
    private static readonly PropertySymbol s_msgProperty = PredefinedSymbols.Err.GetProperty("msg")
        ?? throw new UnreachableException($"Missing property 'msg'");
    private readonly PrimValue _value;

    public ErrorValue(ErrorTypeSymbol errorType, PrimValue value) : base(errorType)
    {
        _value = value;

        var implicit1 = errorType.GetConversion(errorType.ValueType, errorType)
            ?? throw new UnreachableException($"Missing conversion from {errorType.ValueType} to {errorType}");
        Set(
            implicit1,
            new LambdaValue(implicit1.LambdaType, (PrimValue x) => new ErrorValue(errorType, x)));
        var implicit2 = errorType.GetConversion(PredefinedSymbols.Err, errorType)
            ?? throw new UnreachableException($"Missing conversion from {PredefinedSymbols.Err} to {errorType}");
        Set(
            implicit2,
            new LambdaValue(implicit2.LambdaType, (PrimValue x) => new ErrorValue(errorType, x)));
    }

    public override PrimValue Value { get => _value ?? Unit; }

    public bool IsError => _value.Type.IsErr;
    public string? ErrorMessage => !IsError ? null : _value.Get(s_msgProperty).Value as string;

    public bool Equals(ErrorValue? other) => Type == other?.Type && Value.Equals(other.Value);
    public override int GetHashCode() => HashCode.Combine(Type, Value);
}
