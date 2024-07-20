using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class OptionValue : PrimValue
{
    private PrimValue? _value;

    public OptionValue(OptionTypeSymbol optionType) : this(optionType, null) { }
    public OptionValue(PrimValue value) : this(new OptionTypeSymbol(value.Type.Syntax, value.Type), value) { }

    private OptionValue(OptionTypeSymbol optionType, PrimValue? value = null) : base(optionType)
    {
        _value = value;

        var coalesce1 = optionType.GetOperator(
            SyntaxKind.HookHookToken,
            new LambdaTypeSymbol([new Parameter("x", optionType), new Parameter("y", optionType)], optionType))
            ?? throw new UnreachableException($"Missing operator {SyntaxKind.HookHookToken}");
        Set(
            coalesce1,
            new LambdaValue(coalesce1.LambdaType, (PrimValue x, PrimValue y) => ((OptionValue)x).HasValue ? ((OptionValue)x).Value : y));
        var coalesce2 = optionType.GetOperator(
            SyntaxKind.HookHookToken,
            new LambdaTypeSymbol([new Parameter("x", optionType), new Parameter("y", optionType.UnderlyingType)], optionType.UnderlyingType))
            ?? throw new UnreachableException($"Missing operator {SyntaxKind.HookHookToken}");
        Set(
            coalesce2,
            new LambdaValue(coalesce2.LambdaType, (PrimValue x, PrimValue y) => ((OptionValue)x).HasValue ? ((OptionValue)x).Value : y));
        var implicit1 = optionType.GetConversion(optionType.UnderlyingType, optionType)
            ?? throw new UnreachableException($"Missing conversion from {optionType.UnderlyingType} to {optionType}");
        Set(
            implicit1,
            new LambdaValue(implicit1.LambdaType, (PrimValue x) => new OptionValue(x)));
        var implicit2 = optionType.GetConversion(PredefinedSymbols.Unit, optionType)
            ?? throw new UnreachableException($"Missing conversion from {PredefinedSymbols.Unit} to {optionType}");
        Set(
            implicit2,
            new LambdaValue(implicit2.LambdaType, (PrimValue x) => new OptionValue(optionType)));
        var @explicit = optionType.GetConversion(optionType, optionType.UnderlyingType)
            ?? throw new UnreachableException($"Missing conversion from {optionType} to {optionType.UnderlyingType}");
        Set(
            @explicit,
            new LambdaValue(@explicit.LambdaType, (PrimValue x) => ((OptionValue)x).HasValue ? ((OptionValue)x).Value : throw new InvalidOperationException("Invalid cast")));
    }

    public override PrimValue Value { get => _value ?? Unit; }

    public bool HasValue => _value is not null;

    public void SetValue(PrimValue value) => _value = value;
}
