using CodeAnalysis.Syntax;

namespace CodeAnalysis.Types;

public sealed record class ArrayType : PrimType
{
    public ArrayType(PrimType elementType, int length) : base($"[{elementType.Name}: {length}]")
    {
        ElementType = elementType;
        Length = length;
        AddOperator(
            SyntaxKind.BracketOpenBracketCloseToken,
            new FunctionType([new("index", PredefinedTypes.I32)], ElementType));
        //AddOperator(
        //    SyntaxKind.IndexOperator,
        //    new FunctionType([new("index", PredefinedTypes.I32), new("value", ElementType)], PredefinedTypes.Unit));
    }

    public PrimType ElementType { get; init; }
    public int Length { get; init; }

    public bool Equals(ArrayType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
