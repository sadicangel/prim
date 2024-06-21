using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class FunctionValue(FunctionType FunctionType, Delegate Delegate) : PrimValue(FunctionType)
{
    public override Delegate Value => Delegate;

    public PrimValue Invoke(params ReadOnlySpan<PrimValue> arguments) =>
        (PrimValue)Delegate.DynamicInvoke(arguments.ToArray())!;
}
