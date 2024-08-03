using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class OptionValue : PrimValue
{
    private readonly PrimValue _value;

    public OptionValue(OptionTypeSymbol optionType, PrimValue value) : base(optionType)
    {
        _value = value;

        var coalesce1 = optionType.GetOperator(
            SyntaxKind.HookHookToken,
            optionType.ContainingModule.CreateLambdaType([new Parameter("x", optionType), new Parameter("y", optionType)], optionType))
            ?? throw new UnreachableException($"Missing operator {SyntaxKind.HookHookToken}");
        Add(
            coalesce1,
            new LambdaValue(coalesce1.LambdaType, (PrimValue x, PrimValue y) => ((OptionValue)x).HasValue ? ((OptionValue)x).Value : y));
        var coalesce2 = optionType.GetOperator(
            SyntaxKind.HookHookToken,
            optionType.ContainingModule.CreateLambdaType([new Parameter("x", optionType), new Parameter("y", optionType.UnderlyingType)], optionType.UnderlyingType))
            ?? throw new UnreachableException($"Missing operator {SyntaxKind.HookHookToken}");
        Add(
            coalesce2,
            new LambdaValue(coalesce2.LambdaType, (PrimValue x, PrimValue y) => ((OptionValue)x).HasValue ? ((OptionValue)x).Value : y));
    }

    public override PrimValue Value { get => _value; }

    public bool HasValue => !_value.Type.IsUnit;

    public bool Equals(OptionValue? other) => Type == other?.Type && (HasValue ? (other.HasValue && Value.Equals(other.Value)) : !other.HasValue);
    public override int GetHashCode() => HashCode.Combine(Type, HasValue, Value);
}
