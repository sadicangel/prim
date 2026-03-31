using System.Collections;
using System.Runtime.CompilerServices;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Evaluation.Values;

internal sealed record class ArrayValue : PrimValue, IEnumerable<PrimValue>
{
    public ArrayValue(ArrayTypeSymbol arrayType, PrimValue[] elements) : base(arrayType)
    {
        ArrayType = arrayType;
        Elements = elements;
        // index = arrayType.GetOperators(SyntaxKind.BracketOpenBracketCloseToken).Single();
        //Add(index, new LambdaValue(index.LambdaType, GetValue));
    }

    public override PrimValue[] Value => Elements;

    public int Length => Elements.Length;

    public ArrayTypeSymbol ArrayType { get; init; }

    public PrimValue[] Elements { get; init; }

    public PrimValue this[PrimValue index] { get => GetValue(index); set => SetValue(index, value); }

    public bool Equals(ArrayValue? other) => ReferenceEquals(this, other);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    private PrimValue GetValue(PrimValue index) => Elements[(int)index.Value];
    private PrimValue SetValue(PrimValue index, PrimValue value) => Elements[(int)index.Value] = value;

    public IEnumerator<PrimValue> GetEnumerator() => ((IReadOnlyList<PrimValue>)Elements).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
