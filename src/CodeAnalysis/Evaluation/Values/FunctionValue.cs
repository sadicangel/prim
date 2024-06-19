using CodeAnalysis.Types;

namespace CodeAnalysis.Evaluation.Values;

internal sealed record class FunctionValue(PrimType Type, Delegate Delegate) : PrimValue(Type)
{
    public override Delegate Value => Delegate;

    public PrimValue Invoke(params ReadOnlySpan<PrimValue> arguments) =>
        (PrimValue)Delegate.DynamicInvoke(arguments.ToArray())!;
}
