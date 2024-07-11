using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class OptionValue : PrimValue
{
    private PrimValue? _value;

    public OptionValue(OptionType optionType) : this(optionType, null) { }
    public OptionValue(PrimValue value) : this(new OptionType(value.Type), value) { }

    private OptionValue(OptionType optionType, PrimValue? value = null) : base(optionType)
    {
        _value = value;

        var containingSymbol = TypeSymbol.FromType(optionType, NamespaceSymbol.Global);

        var coalesce1 = optionType.GetOperator(
            SyntaxKind.HookHookToken,
            new FunctionType([new Parameter("x", optionType), new Parameter("y", optionType)], optionType))
            ?? throw new UnreachableException($"Missing operator {SyntaxKind.HookHookToken}");
        Set(
            MethodSymbol.FromOperator(coalesce1, containingSymbol),
            new FunctionValue(coalesce1.Type, (PrimValue x, PrimValue y) => ((OptionValue)x).HasValue ? ((OptionValue)x).Value : y));
        var coalesce2 = optionType.GetOperator(
            SyntaxKind.HookHookToken,
            new FunctionType([new Parameter("x", optionType), new Parameter("y", optionType.UnderlyingType)], optionType.UnderlyingType))
            ?? throw new UnreachableException($"Missing operator {SyntaxKind.HookHookToken}");
        Set(
            MethodSymbol.FromOperator(coalesce2, containingSymbol),
            new FunctionValue(coalesce2.Type, (PrimValue x, PrimValue y) => ((OptionValue)x).HasValue ? ((OptionValue)x).Value : y));
        var @implicit = optionType.GetConversion(optionType.UnderlyingType, optionType)
            ?? throw new UnreachableException($"Missing conversion from {optionType.UnderlyingType} to {optionType}");
        Set(
            MethodSymbol.FromConversion(@implicit, containingSymbol),
            new FunctionValue(@implicit.Type, (PrimValue x) => new OptionValue(x)));
        var @explicit = optionType.GetConversion(optionType, optionType.UnderlyingType)
            ?? throw new UnreachableException($"Missing conversion from {optionType} to {optionType.UnderlyingType}");
        Set(
            MethodSymbol.FromConversion(@explicit, containingSymbol),
            new FunctionValue(@explicit.Type, (PrimValue x) => ((OptionValue)x).HasValue ? ((OptionValue)x).Value : throw new InvalidOperationException("Invalid cast")));
    }

    public override PrimValue Value { get => _value ?? Unit; }

    public bool HasValue => _value is not null;

    public void SetValue(PrimValue value) => _value = value;
}
