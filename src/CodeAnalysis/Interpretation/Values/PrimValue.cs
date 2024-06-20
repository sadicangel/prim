using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;
public abstract record class PrimValue(PrimType Type)
{
    public abstract object Value { get; }
}
