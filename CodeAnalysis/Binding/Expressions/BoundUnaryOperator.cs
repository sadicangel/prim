using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundUnaryOperator(TokenKind TokenKind, BoundUnaryOperatorKind Kind, TypeSymbol OperandType, TypeSymbol ResultType)
{
    private static readonly BoundUnaryOperator[] Operators =
    {
        new BoundUnaryOperator(TokenKind.Bang, BoundUnaryOperatorKind.Not, BuiltinTypes.Bool),
        new BoundUnaryOperator(TokenKind.Minus, BoundUnaryOperatorKind.Negate, BuiltinTypes.I32),
        new BoundUnaryOperator(TokenKind.Plus, BoundUnaryOperatorKind.UnaryPlus, BuiltinTypes.I32),
        new BoundUnaryOperator(TokenKind.Tilde, BoundUnaryOperatorKind.OnesComplement, BuiltinTypes.I32),
    };

    public BoundUnaryOperator(TokenKind tokenKind, BoundUnaryOperatorKind kind, TypeSymbol operandType)
        : this(tokenKind, kind, operandType, operandType)
    {

    }

    public static BoundUnaryOperator? Bind(TokenKind tokenKind, TypeSymbol operandType)
    {
        foreach (var @operator in Operators)
        {
            if (@operator.TokenKind == tokenKind && @operator.OperandType == operandType)
                return @operator;
        }

        return null;
    }
}