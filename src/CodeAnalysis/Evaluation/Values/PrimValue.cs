using CodeAnalysis.Types;

namespace CodeAnalysis.Evaluation.Values;
public abstract record class PrimValue(PrimType Type)
{
    public abstract object Value { get; }
}
