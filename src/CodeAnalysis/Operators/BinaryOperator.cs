using CodeAnalysis.Types;
using System.Diagnostics;

namespace CodeAnalysis.Operators;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record class BinaryOperator(
    BinaryOperatorKind OperatorKind,
    PrimType LeftType,
    PrimType RightType,
    PrimType ResultType
)
    : Operator(ResultType)
{
    private string GetDebuggerDisplay() => $"operator {OperatorKind}: (a: {LeftType.Name}, b: {RightType.Name}) -> {ResultType.Name}";
}

public enum BinaryOperatorKind
{
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
    ExplicitCast,
    ImplicitCast,
}