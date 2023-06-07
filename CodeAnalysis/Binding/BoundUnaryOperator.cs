using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundUnaryOperator(TokenKind TokenKind, BoundUnaryOperatorKind Kind, Type OperandType, Type ResultType) : BoundOperator
{
    private static readonly BoundUnaryOperator[] Operators =
    {
        new BoundUnaryOperator(TokenKind.Bang, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
        new BoundUnaryOperator(TokenKind.Minus, BoundUnaryOperatorKind.Negation, typeof(int)),
        new BoundUnaryOperator(TokenKind.Plus, BoundUnaryOperatorKind.Identity, typeof(int)),
        new BoundUnaryOperator(TokenKind.Tilde, BoundUnaryOperatorKind.OnesComplement, typeof(int)),
    };

    public BoundUnaryOperator(TokenKind tokenKind, BoundUnaryOperatorKind kind, Type operandType)
        : this(tokenKind, kind, operandType, operandType)
    {

    }

    public static BoundUnaryOperator? Bind(TokenKind tokenKind, Type operandType)
    {
        foreach (var @operator in Operators)
        {
            if (@operator.TokenKind == tokenKind && @operator.OperandType == operandType)
                return @operator;
        }

        return null;
    }

    protected override string GetDisplayString() => $"{ResultType} operator{TokenKind.GetText()}({OperandType})";
}