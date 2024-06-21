using System.Collections;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class ArrayValue(ArrayType ArrayType, PrimValue[] Elements)
    : PrimValue(ArrayType), IEnumerable<PrimValue>
{
    public override PrimValue[] Value => Elements;

    public int Length => ArrayType.Length;

    public PrimValue this[PrimValue index] { get => Elements[(int)index.Value]; set => Elements[(int)index.Value] = value; }

    public IEnumerator<PrimValue> GetEnumerator()
    {
        foreach (var element in Elements)
            yield return element;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
