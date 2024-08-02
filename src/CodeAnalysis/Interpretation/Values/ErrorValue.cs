using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class ErrorValue : PrimValue
{
    public ErrorValue(ErrorTypeSymbol errorType, PrimValue value) : base(errorType)
    {
        Value = value;

        var implicit1 = errorType.GetConversion(errorType.ValueType, errorType)
            ?? throw new UnreachableException($"Missing conversion from {errorType.ValueType} to {errorType}");
        Add(
            implicit1,
            new LambdaValue(implicit1.LambdaType, (PrimValue x) => new ErrorValue(errorType, x)));
        var implicit2 = errorType.GetConversion(errorType, errorType)
            ?? throw new UnreachableException($"Missing conversion from {errorType} to {errorType}");
        Add(
            implicit2,
            new LambdaValue(implicit2.LambdaType, (PrimValue x) => new ErrorValue(errorType, x)));
    }

    public override PrimValue Value { get; }

    public bool IsError => Value.Type.IsErr;
    public string? ErrorMessage => !IsError ? null : Value.Get(Value.Type.GetProperty("msg")!).Value as string;

    public bool Equals(ErrorValue? other) => Type == other?.Type && Value.Equals(other.Value);
    public override int GetHashCode() => HashCode.Combine(Type, Value);
}
