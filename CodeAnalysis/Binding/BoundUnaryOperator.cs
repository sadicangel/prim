using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundUnaryOperator(TokenKind TokenKind, BoundUnaryOperatorKind Kind, TypeSymbol OperandType, TypeSymbol ResultType)
{
    private static readonly BoundUnaryOperator[] Operators =
    {
        new BoundUnaryOperator(TokenKind.Bang, BoundUnaryOperatorKind.LogicalNegation, TypeSymbol.Bool),
        new BoundUnaryOperator(TokenKind.Minus, BoundUnaryOperatorKind.Negation, TypeSymbol.I32),
        new BoundUnaryOperator(TokenKind.Plus, BoundUnaryOperatorKind.Identity, TypeSymbol.I32),
        new BoundUnaryOperator(TokenKind.Tilde, BoundUnaryOperatorKind.OnesComplement, TypeSymbol.I32),
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