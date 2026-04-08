using System.Diagnostics;
using CodeAnalysis.Syntax;
using DotNext.Buffers;

namespace CodeAnalysis.Semantic.Symbols;

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
    GreaterThanOrEqual
}

internal static class OperatorKindExtensions
{
    extension(OperatorKind operatorKind)
    {
        public string GetOperatorName(params ReadOnlySpan<TypeSymbol> operands)
        {
            using var writer = new BufferWriterSlim<char>(stackalloc char[256]);
            foreach (var operand in operands)
            {
                if (writer.WrittenCount > 0) writer.Write([',']);
                writer.Write(operand.FullName);
            }

            return $"{operatorKind.GetMethodName()}<{writer.WrittenSpan.ToString()}>";
        }

        private string GetMethodName() => operatorKind switch
        {
            OperatorKind.UnaryPlus => "op_UnaryPlus",
            OperatorKind.UnaryMinus => "op_UnaryNegation",
            OperatorKind.OnesComplement => "op_OnesComplement",
            OperatorKind.LogicalNot => "op_LogicalNot",
            OperatorKind.Addition => "op_Addition",
            OperatorKind.Subtraction => "op_Subtraction",
            OperatorKind.Multiplication => "op_Multiply",
            OperatorKind.Division => "op_Division",
            OperatorKind.Modulo => "op_Modulus",
            OperatorKind.Exponentiation => "op_Exponentiation",
            OperatorKind.LeftShift => "op_LeftShift",
            OperatorKind.RightShift => "op_RightShift",
            OperatorKind.LogicalOr => "op_LogicalOr",
            OperatorKind.LogicalAnd => "op_LogicalAnd",
            OperatorKind.BitwiseOr => "op_BitwiseOr",
            OperatorKind.BitwiseAnd => "op_BitwiseAnd",
            OperatorKind.ExclusiveOr => "op_ExclusiveOr",
            OperatorKind.Equals => "op_Equality",
            OperatorKind.NotEquals => "op_Inequality",
            OperatorKind.LessThan => "op_LessThan",
            OperatorKind.LessThanOrEqual => "op_LessThanOrEqual",
            OperatorKind.GreaterThan => "op_GreaterThan",
            OperatorKind.GreaterThanOrEqual => "op_GreaterThanOrEqual",
            _ => throw new UnreachableException($"Unexpected operator kind '{operatorKind}'"),
        };
    }

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
