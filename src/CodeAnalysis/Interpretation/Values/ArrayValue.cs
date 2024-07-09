using System.Collections;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;
internal sealed record class ArrayValue
    : PrimValue, IEnumerable<PrimValue>
{
    public ArrayValue(ArrayType arrayType, PrimValue[] elements) : base(arrayType)
    {
        ArrayType = arrayType;
        Elements = elements;
        var index = arrayType.GetOperators(SyntaxKind.BracketOpenBracketCloseToken).Single();
        Set(
            FunctionSymbol.FromOperator(index),
            new FunctionValue(
                index.Type,
                GetValue));
    }

    public override PrimValue[] Value => Elements;

    public int Length => ArrayType.Length;

    public ArrayType ArrayType { get; init; }

    public PrimValue[] Elements { get; init; }

    public PrimValue this[PrimValue index] { get => GetValue(index); set => SetValue(index, value); }

    private PrimValue GetValue(PrimValue index) => Elements[(int)index.Value];
    private PrimValue SetValue(PrimValue index, PrimValue value) => Elements[(int)index.Value] = value;

    public IEnumerator<PrimValue> GetEnumerator()
    {
        foreach (var element in Elements)
            yield return element;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
