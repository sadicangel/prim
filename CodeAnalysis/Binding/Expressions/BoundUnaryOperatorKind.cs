namespace CodeAnalysis.Binding.Expressions;

internal enum BoundUnaryOperatorKind
{
    UnaryPlus,
    Negate,
    Increment,
    Decrement,
    Not,
    OnesComplement
}

internal static class BoundUnaryOperatorKindExtensions
{
    public static string GetLinqExpressionName(this BoundUnaryOperatorKind kind) => kind switch
    {
        _ => kind.ToString()
    };
}