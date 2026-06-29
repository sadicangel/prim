using System.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal enum OperatorKind
{
    UnaryPlus,
    UnaryMinus,
    OnesComplement,
    LogicalNot,
    Addition,
    Subtraction,
    Multiplication,
    Division,
    Modulo,
    Exponentiation,
    LeftShift,
    RightShift,
    LogicalOr,
    LogicalAnd,
    BitwiseOr,
    BitwiseAnd,
    ExclusiveOr,
    Equals,
    NotEquals,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    Index,
}

internal static class OperatorKindExtensions
{
    extension(SyntaxKind syntaxKind)
    {
        public OperatorKind GetOperatorKind(int operandCount) => operandCount switch
        {
            1 => syntaxKind.GetUnaryOperatorKind(),
            2 => syntaxKind.GetBinaryOperatorKind(),
            _ => throw new UnreachableException($"Unexpected number of operands '{operandCount}' for operator"),
        };

        private OperatorKind GetUnaryOperatorKind() => syntaxKind switch
        {
            SyntaxKind.PlusToken => OperatorKind.UnaryPlus,
            SyntaxKind.MinusToken => OperatorKind.UnaryMinus,
            SyntaxKind.TildeToken => OperatorKind.OnesComplement,
            SyntaxKind.ExclamationToken => OperatorKind.LogicalNot,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntaxKind}' for operator"),
        };

        private OperatorKind GetBinaryOperatorKind() => syntaxKind switch
        {
            SyntaxKind.PlusToken => OperatorKind.Addition,
            SyntaxKind.MinusToken => OperatorKind.Subtraction,
            SyntaxKind.AsteriskToken => OperatorKind.Multiplication,
            SyntaxKind.SlashToken => OperatorKind.Division,
            SyntaxKind.PercentToken => OperatorKind.Modulo,
            SyntaxKind.AsteriskAsteriskToken => OperatorKind.Exponentiation,
            SyntaxKind.LessThanLessThanToken => OperatorKind.LeftShift,
            SyntaxKind.GreaterThanGreaterThanToken => OperatorKind.RightShift,
            SyntaxKind.BarBarToken => OperatorKind.LogicalOr,
            SyntaxKind.AmpersandAmpersandToken => OperatorKind.LogicalAnd,
            SyntaxKind.BarToken => OperatorKind.BitwiseOr,
            SyntaxKind.AmpersandToken => OperatorKind.BitwiseAnd,
            SyntaxKind.CaretToken => OperatorKind.ExclusiveOr,
            SyntaxKind.EqualsEqualsToken => OperatorKind.Equals,
            SyntaxKind.ExclamationEqualsToken => OperatorKind.NotEquals,
            SyntaxKind.LessThanToken => OperatorKind.LessThan,
            SyntaxKind.LessThanEqualsToken => OperatorKind.LessThanOrEqual,
            SyntaxKind.GreaterThanToken => OperatorKind.GreaterThan,
            SyntaxKind.GreaterThanEqualsToken => OperatorKind.GreaterThanOrEqual,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntaxKind}' for operator"),
        };
    }
}
