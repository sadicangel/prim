namespace CodeAnalysis.Types;

public sealed record class TypeList(IReadOnlyList<PrimType> Types)
    : PrimType(String.Join(", ", Types.Select(t => t.Name)))
{
    public override bool IsAssignableFrom(PrimType source)
    {
        if (source is not TypeList other)
            return false;

        for (int i = 0; i < Types.Count; ++i)
            if (!Types[i].IsAssignableFrom(other.Types[i]))
                return false;

        return true;
    }
}