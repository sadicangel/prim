using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundBinaryOperator(TokenKind TokenKind, BoundBinaryOperatorKind Kind, TypeSymbol LeftType, TypeSymbol RightType, TypeSymbol ResultType)
{
    private static readonly BoundBinaryOperator[] Operators =
    {
        // cast operator
        new BoundBinaryOperator(TokenKind.As, BoundBinaryOperatorKind.ExplicitCast, BuiltinTypes.Any, BuiltinTypes.Type, BuiltinTypes.Any),

        // i32 operators
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Add, BuiltinTypes.I32),
        new BoundBinaryOperator(TokenKind.Minus, BoundBinaryOperatorKind.Subtract, BuiltinTypes.I32),
        new BoundBinaryOperator(TokenKind.Star, BoundBinaryOperatorKind.Multiply, BuiltinTypes.I32),
        new BoundBinaryOperator(TokenKind.Slash, BoundBinaryOperatorKind.Divide, BuiltinTypes.I32),
        new BoundBinaryOperator(TokenKind.Percent, BoundBinaryOperatorKind.Modulo, BuiltinTypes.I32),

        new BoundBinaryOperator(TokenKind.Ampersand, BoundBinaryOperatorKind.And, BuiltinTypes.I32),
        new BoundBinaryOperator(TokenKind.Pipe, BoundBinaryOperatorKind.Or, BuiltinTypes.I32),
        new BoundBinaryOperator(TokenKind.Hat, BoundBinaryOperatorKind.ExclusiveOr, BuiltinTypes.I32),

        new BoundBinaryOperator(TokenKind.EqualEqual, BoundBinaryOperatorKind.Equal, BuiltinTypes.I32, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.EqualEqual, BoundBinaryOperatorKind.Equal, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.BangEqual, BoundBinaryOperatorKind.NotEqual, BuiltinTypes.I32, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.BangEqual, BoundBinaryOperatorKind.NotEqual, BuiltinTypes.Bool),

        new BoundBinaryOperator(TokenKind.Less, BoundBinaryOperatorKind.LessThan, BuiltinTypes.I32, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.LessEqual, BoundBinaryOperatorKind.LessThanOrEqual, BuiltinTypes.I32, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.Greater, BoundBinaryOperatorKind.GreaterThan, BuiltinTypes.I32, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.GreaterEqual, BoundBinaryOperatorKind.GreaterThanOrEqual, BuiltinTypes.I32, BuiltinTypes.Bool),

        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Add, BuiltinTypes.F32),
        new BoundBinaryOperator(TokenKind.Minus, BoundBinaryOperatorKind.Subtract, BuiltinTypes.F32),
        new BoundBinaryOperator(TokenKind.Star, BoundBinaryOperatorKind.Multiply, BuiltinTypes.F32),
        new BoundBinaryOperator(TokenKind.Slash, BoundBinaryOperatorKind.Divide, BuiltinTypes.F32),
        new BoundBinaryOperator(TokenKind.Percent, BoundBinaryOperatorKind.Modulo, BuiltinTypes.F32),
        
        // f32 operators
        new BoundBinaryOperator(TokenKind.EqualEqual, BoundBinaryOperatorKind.Equal, BuiltinTypes.F32, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.EqualEqual, BoundBinaryOperatorKind.Equal, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.BangEqual, BoundBinaryOperatorKind.NotEqual, BuiltinTypes.F32, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.BangEqual, BoundBinaryOperatorKind.NotEqual, BuiltinTypes.Bool),

        new BoundBinaryOperator(TokenKind.Less, BoundBinaryOperatorKind.LessThan, BuiltinTypes.F32, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.LessEqual, BoundBinaryOperatorKind.LessThanOrEqual, BuiltinTypes.F32, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.Greater, BoundBinaryOperatorKind.GreaterThan, BuiltinTypes.F32, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.GreaterEqual, BoundBinaryOperatorKind.GreaterThanOrEqual, BuiltinTypes.F32, BuiltinTypes.Bool),
        
        // bool operators
        new BoundBinaryOperator(TokenKind.AmpersandAmpersand, BoundBinaryOperatorKind.AndAlso, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.PipePipe, BoundBinaryOperatorKind.OrElse, BuiltinTypes.Bool),

        new BoundBinaryOperator(TokenKind.Ampersand, BoundBinaryOperatorKind.And, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.Pipe, BoundBinaryOperatorKind.Or, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.Hat, BoundBinaryOperatorKind.ExclusiveOr, BuiltinTypes.Bool),
        
        // str operators
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Add, BuiltinTypes.Str),
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Add, BuiltinTypes.Str, BuiltinTypes.Any, BuiltinTypes.Str),
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Add, BuiltinTypes.Any, BuiltinTypes.Str, BuiltinTypes.Str),

        new BoundBinaryOperator(TokenKind.EqualEqual, BoundBinaryOperatorKind.Equal, BuiltinTypes.Str, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.EqualEqual, BoundBinaryOperatorKind.Equal, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.BangEqual, BoundBinaryOperatorKind.NotEqual, BuiltinTypes.Str, BuiltinTypes.Bool),
        new BoundBinaryOperator(TokenKind.BangEqual, BoundBinaryOperatorKind.NotEqual, BuiltinTypes.Bool),
    };

    public BoundBinaryOperator(TokenKind tokenKind, BoundBinaryOperatorKind kind, TypeSymbol operandType)
        : this(tokenKind, kind, operandType, operandType, operandType)
    {
    }

    public BoundBinaryOperator(TokenKind tokenKind, BoundBinaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
        : this(tokenKind, kind, operandType, operandType, resultType)
    {

    }

    public static BoundBinaryOperator? Bind(TokenKind tokenKind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol? resultType)
    {
        var matchingOperator = default(BoundBinaryOperator);
        foreach (var @operator in Operators)
        {
            if (@operator.TokenKind == tokenKind &&
                @operator.LeftType.IsAssignableFrom(leftType) &&
                @operator.RightType.IsAssignableFrom(rightType) &&
                (resultType is null || @operator.ResultType.IsAssignableFrom(resultType)))
            {
                matchingOperator = @operator;
                break;
            }
        }

        if (matchingOperator?.Kind is BoundBinaryOperatorKind.ExplicitCast or BoundBinaryOperatorKind.ImplicitCast)
        {
            if (resultType is null)
                return null;

            var conversion = Conversion.Classify(leftType, resultType);
            if (!conversion.Exists)
                return null;

            //if(!conversion.IsIdentity)
            //Report unnecessary conversion.

            matchingOperator = matchingOperator with
            {
                LeftType = leftType,
                ResultType = resultType,
            };
        }

        return matchingOperator;
    }
}