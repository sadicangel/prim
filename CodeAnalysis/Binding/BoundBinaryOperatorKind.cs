using System.Linq.Expressions;

namespace CodeAnalysis.Binding;

internal enum BoundBinaryOperatorKind
{
    Add,
    Subtract,
    Multiply,
    Divide,
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
    Modulo,
    AndAlso,
    OrElse,
    ExplicitCast,
    ImplicitCast,
}

internal static class BoundBinaryOperatorKindExtensions
{
    public static string GetLinqExpressionName(this BoundBinaryOperatorKind kind) => kind switch
    {
        BoundBinaryOperatorKind.ExplicitCast or BoundBinaryOperatorKind.ImplicitCast => nameof(Expression.Convert),
        _ => kind.ToString(),
    };
}