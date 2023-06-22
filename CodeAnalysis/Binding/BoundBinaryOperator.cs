using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundBinaryOperator(TokenKind TokenKind, BoundBinaryOperatorKind Kind, TypeSymbol LeftType, TypeSymbol RightType, TypeSymbol ResultType)
{
    private static readonly BoundBinaryOperator[] Operators =
    {
        // i32 operators
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Add, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Minus, BoundBinaryOperatorKind.Subtract, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Star, BoundBinaryOperatorKind.Multiply, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Slash, BoundBinaryOperatorKind.Divide, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Percent, BoundBinaryOperatorKind.Modulo, TypeSymbol.I32),

        new BoundBinaryOperator(TokenKind.Ampersand, BoundBinaryOperatorKind.And, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Pipe, BoundBinaryOperatorKind.Or, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Hat, BoundBinaryOperatorKind.ExclusiveOr, TypeSymbol.I32),

        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equal, TypeSymbol.I32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equal, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEqual, TypeSymbol.I32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEqual, TypeSymbol.Bool),

        new BoundBinaryOperator(TokenKind.Less, BoundBinaryOperatorKind.LessThan, TypeSymbol.I32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.LessEquals, BoundBinaryOperatorKind.LessThanOrEqual, TypeSymbol.I32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.Greater, BoundBinaryOperatorKind.GreaterThan, TypeSymbol.I32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.GreaterEquals, BoundBinaryOperatorKind.GreaterThanOrEqual, TypeSymbol.I32, TypeSymbol.Bool),

        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Add, TypeSymbol.F32),
        new BoundBinaryOperator(TokenKind.Minus, BoundBinaryOperatorKind.Subtract, TypeSymbol.F32),
        new BoundBinaryOperator(TokenKind.Star, BoundBinaryOperatorKind.Multiply, TypeSymbol.F32),
        new BoundBinaryOperator(TokenKind.Slash, BoundBinaryOperatorKind.Divide, TypeSymbol.F32),
        new BoundBinaryOperator(TokenKind.Percent, BoundBinaryOperatorKind.Modulo, TypeSymbol.F32),
        
        // f32 operators
        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equal, TypeSymbol.F32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equal, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEqual, TypeSymbol.F32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEqual, TypeSymbol.Bool),

        new BoundBinaryOperator(TokenKind.Less, BoundBinaryOperatorKind.LessThan, TypeSymbol.F32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.LessEquals, BoundBinaryOperatorKind.LessThanOrEqual, TypeSymbol.F32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.Greater, BoundBinaryOperatorKind.GreaterThan, TypeSymbol.F32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.GreaterEquals, BoundBinaryOperatorKind.GreaterThanOrEqual, TypeSymbol.F32, TypeSymbol.Bool),
        
        // bool operators
        new BoundBinaryOperator(TokenKind.AmpersandAmpersand, BoundBinaryOperatorKind.AndAlso, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.PipePipe, BoundBinaryOperatorKind.OrElse, TypeSymbol.Bool),

        new BoundBinaryOperator(TokenKind.Ampersand, BoundBinaryOperatorKind.And, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.Pipe, BoundBinaryOperatorKind.Or, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.Hat, BoundBinaryOperatorKind.ExclusiveOr, TypeSymbol.Bool),
        
        // str operators
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Add, TypeSymbol.String),
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Add, TypeSymbol.String, TypeSymbol.Any, TypeSymbol.String),
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Add, TypeSymbol.Any, TypeSymbol.String, TypeSymbol.String),

        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equal, TypeSymbol.String, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equal, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEqual, TypeSymbol.String, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEqual, TypeSymbol.Bool),
    };

    public BoundBinaryOperator(TokenKind tokenKind, BoundBinaryOperatorKind kind, TypeSymbol operandType)
        : this(tokenKind, kind, operandType, operandType, operandType)
    {
    }

    public BoundBinaryOperator(TokenKind tokenKind, BoundBinaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
        : this(tokenKind, kind, operandType, operandType, resultType)
    {

    }

    public static BoundBinaryOperator? Bind(TokenKind tokenKind, TypeSymbol leftType, TypeSymbol rightType)
    {
        var matchingOperators = new List<BoundBinaryOperator>();
        foreach (var @operator in Operators)
        {
            if (@operator.TokenKind == tokenKind && @operator.LeftType.IsAssignableFrom(leftType) && @operator.RightType.IsAssignableFrom(rightType))
                matchingOperators.Add(@operator);
        }

        return matchingOperators.FirstOrDefault();
    }
}