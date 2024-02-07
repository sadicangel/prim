using CodeAnalysis.Types;

namespace CodeAnalysis.Operators;
public abstract record class Operator(OperatorKind OperatorKind, PrimType ResultType);

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
