using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Operators;
public abstract record class Operator(OperatorKind OperatorKind, PrimType ResultType) : Member($"operator {OperatorKind}");

public sealed record class UnaryOperatorInfo(
    OperatorKind OperatorKind,
    PrimType OperandType,
    PrimType ResultType
)
    : Operator(OperatorKind, ResultType)
{
    public override string ToString() => $"operator {OperatorKind}: (a: {OperandType.Name}) -> {ResultType.Name}";
}

public sealed record class BinaryOperatorInfo(
    OperatorKind OperatorKind,
    PrimType LeftType,
    PrimType RightType,
    PrimType ResultType
)
    : Operator(OperatorKind, ResultType)
{
    public override string ToString() => $"operator {OperatorKind}: (a: {LeftType.Name}, b: {RightType.Name}) -> {ResultType.Name}";
}

public enum OperatorKind
{
    UnaryPlus,
    Negate,
    PrefixIncrement,
    PrefixDecrement,
    OnesComplement,
    Not,

    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
    Exponent,

    And,
    Or,
    ExclusiveOr,
    LeftShift,
    RightShift,

    Equal,
    NotEqual,

    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,

    AndAlso,
    OrElse,

    ExplicitConversion,
    ImplicitConversion,

    NullCoalescence,
    Call,
    Subscript,
    Reference,
}
