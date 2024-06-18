using CodeAnalysis.Types;

namespace CodeAnalysis.Evaluation.Values;

internal sealed record class FunctionValue(PrimType Type, Delegate Value) : PrimValue(Type, Value)
{
    public override Delegate Value { get; } = Value;

    public PrimValue Invoke(params ReadOnlySpan<PrimValue> arguments) =>
        (PrimValue)Value.DynamicInvoke(arguments.ToArray())!;
}
