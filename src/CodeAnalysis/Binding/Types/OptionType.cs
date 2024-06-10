using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Types;

internal sealed record class OptionType : PrimType
{
    public OptionType(PrimType underlyingType) : base($"?{underlyingType.Name}")
    {
        UnderlyingType = underlyingType;
        AddOperator(new Operator(
            SyntaxKind.CoalesceOperator,
            new FunctionType([new Parameter("x", this), new Parameter("y", this)], this)));
        AddOperator(new Operator(
            SyntaxKind.CoalesceOperator,
            new FunctionType([new Parameter("x", this), new Parameter("y", UnderlyingType)], UnderlyingType)));
        AddConversion(new Conversion(
            SyntaxKind.ImplicitKeyword,
            new FunctionType([new Parameter("x", UnderlyingType)], this)));
    }

    public PrimType UnderlyingType { get; init; }

    public bool Equals(OptionType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
