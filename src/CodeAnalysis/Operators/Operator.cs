using CodeAnalysis.Types;

namespace CodeAnalysis.Operators;
public abstract record class Operator(OperatorKind OperatorKind, PrimType ResultType);

public enum OperatorKind
{
    UnaryPlus,
    Negate,
    Increment,
    Decrement,
    Not,
    OnesComplement,

    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
    Exponent,
    And,
    Or,
    ExclusiveOr,
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LeftShift,
    RightShift,
    AndAlso,
    OrElse,
    NullCoalescence,
    Call,
    Subscript,
    Reference,
    ExplicitCast,
    ImplicitCast,
}
