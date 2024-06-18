using CodeAnalysis.Types;

namespace CodeAnalysis.Evaluation.Values;
public abstract record class PrimValue(PrimType Type, object Value)
{
    public virtual object Value { get; } = Value;
}
