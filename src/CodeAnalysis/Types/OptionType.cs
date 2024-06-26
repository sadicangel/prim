﻿using CodeAnalysis.Syntax;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Types;

public sealed record class OptionType : PrimType
{
    public OptionType(PrimType underlyingType) : base($"?{underlyingType.Name}")
    {
        UnderlyingType = underlyingType;
        AddOperator(
            SyntaxKind.CoalesceOperator,
            new FunctionType([new Parameter("x", this), new Parameter("y", this)], this));
        AddOperator(
            SyntaxKind.CoalesceOperator,
            new FunctionType([new Parameter("x", this), new Parameter("y", UnderlyingType)], UnderlyingType));
        AddConversion(
            SyntaxKind.ImplicitKeyword,
            new FunctionType([new Parameter("x", UnderlyingType)], this));
    }

    public PrimType UnderlyingType { get; init; }

    public bool Equals(OptionType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
