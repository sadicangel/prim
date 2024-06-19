using CodeAnalysis.Types;

namespace CodeAnalysis.Evaluation.Values;

internal sealed record class LiteralValue(PrimType Type, object Value) : PrimValue(Type)
{
    public override object Value { get; } = Value;

    public static LiteralValue Unit { get; } = new LiteralValue(PredefinedTypes.Unit, new());
    public static LiteralValue True { get; } = new LiteralValue(PredefinedTypes.Bool, true);
    public static LiteralValue False { get; } = new LiteralValue(PredefinedTypes.Bool, false);
}

