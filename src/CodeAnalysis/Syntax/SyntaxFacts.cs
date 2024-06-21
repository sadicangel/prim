using System.Diagnostics;

namespace CodeAnalysis.Syntax;

public static class SyntaxFacts
{
    public static string? GetText(SyntaxKind syntaxKind)
    {
        return syntaxKind switch
        {
            SyntaxKind.InvalidSyntax => null,
            SyntaxKind.EofToken => null,
            SyntaxKind.IdentifierToken => null,

            SyntaxKind.AmpersandToken => "&",
            SyntaxKind.AmpersandAmpersandToken => "&&",
            SyntaxKind.AmpersandEqualsToken => "&=",
            SyntaxKind.BangToken => "!",
            SyntaxKind.BangEqualsToken => "!=",
            SyntaxKind.BraceOpenToken => "{",
            SyntaxKind.BraceCloseToken => "}",
            SyntaxKind.BracketOpenToken => "[",
            SyntaxKind.BracketCloseToken => "]",
            SyntaxKind.ColonToken => ":",
            SyntaxKind.CommaToken => ",",
            SyntaxKind.DotToken => ".",
            SyntaxKind.DotDotToken => "..",
            SyntaxKind.EqualsToken => "=",
            SyntaxKind.EqualsEqualsToken => "==",
            SyntaxKind.MinusGreaterThanToken => "->",
            SyntaxKind.GreaterThanToken => ">",
            SyntaxKind.GreaterThanEqualsToken => ">=",
            SyntaxKind.GreaterThanGreaterThanToken => ">>",
            SyntaxKind.GreaterThanGreaterThanEqualsToken => ">>=",
            SyntaxKind.HatToken => "^",
            SyntaxKind.HatEqualsToken => "^=",
            SyntaxKind.HookToken => "?",
            SyntaxKind.HookHookToken => "??",
            SyntaxKind.HookHookEqualsToken => "??=",
            SyntaxKind.LessThanToken => "<",
            SyntaxKind.LessThanEqualsToken => "<=",
            SyntaxKind.LessThanLessThanToken => "<<",
            SyntaxKind.LessThanLessThanEqualsToken => "<<=",
            SyntaxKind.MinusToken => "-",
            SyntaxKind.MinusEqualsToken => "-=",
            SyntaxKind.ParenthesisOpenToken => "(",
            SyntaxKind.ParenthesisCloseToken => ")",
            SyntaxKind.PercentToken => "%",
            SyntaxKind.PercentEqualsToken => "%=",
            SyntaxKind.PipeToken => "|",
            SyntaxKind.PipeEqualsToken => "|=",
            SyntaxKind.PipePipeToken => "||",
            SyntaxKind.PlusToken => "+",
            SyntaxKind.PlusEqualsToken => "+=",
            SyntaxKind.SemicolonToken => ";",
            SyntaxKind.SlashToken => "/",
            SyntaxKind.SlashEqualsToken => "/=",
            SyntaxKind.StarToken => "*",
            SyntaxKind.StarEqualsToken => "*=",
            SyntaxKind.StarStarToken => "**",
            SyntaxKind.StarStarEqualsToken => "**=",
            SyntaxKind.TildeToken => "~",

            SyntaxKind.I32LiteralToken => null,
            SyntaxKind.U32LiteralToken => null,
            SyntaxKind.I64LiteralToken => null,
            SyntaxKind.U64LiteralToken => null,
            SyntaxKind.F32LiteralToken => null,
            SyntaxKind.F64LiteralToken => null,
            SyntaxKind.StrLiteralToken => null,

            SyntaxKind.AsKeyword => "as",
            SyntaxKind.IfKeyword => "if",
            SyntaxKind.ImplicitKeyword => "implicit",
            SyntaxKind.ElseKeyword => "else",
            SyntaxKind.ExplicitKeyword => "explicit",
            SyntaxKind.WhileKeyword => "while",
            SyntaxKind.ForKeyword => "for",
            SyntaxKind.ContinueKeyword => "continue",
            SyntaxKind.BreakKeyword => "break",
            SyntaxKind.ReturnKeyword => "return",

            SyntaxKind.AnyKeyword => "any",
            SyntaxKind.UnknownKeyword => "unknown",
            SyntaxKind.NeverKeyword => "never",
            SyntaxKind.UnitKeyword => "unit",
            SyntaxKind.TypeKeyword => "type",
            SyntaxKind.StrKeyword => "str",
            SyntaxKind.BoolKeyword => "bool",
            SyntaxKind.I8Keyword => "i8",
            SyntaxKind.I16Keyword => "i16",
            SyntaxKind.I32Keyword => "i32",
            SyntaxKind.I64Keyword => "i64",
            SyntaxKind.I128Keyword => "i128",
            SyntaxKind.ISizeKeyword => "isize",
            SyntaxKind.U8Keyword => "u8",
            SyntaxKind.U16Keyword => "u16",
            SyntaxKind.U32Keyword => "u32",
            SyntaxKind.U64Keyword => "u64",
            SyntaxKind.U128Keyword => "u128",
            SyntaxKind.USizeKeyword => "usize",
            SyntaxKind.F16Keyword => "f16",
            SyntaxKind.F32Keyword => "f32",
            SyntaxKind.F64Keyword => "f64",
            SyntaxKind.F80Keyword => "f80",
            SyntaxKind.F128Keyword => "f128",

            SyntaxKind.StructKeyword => "struct",

            SyntaxKind.TrueKeyword => "true",
            SyntaxKind.FalseKeyword => "false",
            SyntaxKind.NullKeyword => "null",

            SyntaxKind.LineBreakTrivia => null,
            SyntaxKind.WhiteSpaceTrivia => null,
            SyntaxKind.SingleLineCommentTrivia => null,
            SyntaxKind.MultiLineCommentTrivia => null,
            SyntaxKind.InvalidTextTrivia => null,

            SyntaxKind.CompilationUnit => null,

            SyntaxKind.PredefinedType => null,
            SyntaxKind.NamedType => null,
            SyntaxKind.OptionType => null,
            SyntaxKind.ArrayType => null,
            SyntaxKind.FunctionType => null,
            SyntaxKind.UnionType => null,
            SyntaxKind.Parameter => null,
            SyntaxKind.Argument => null,

            SyntaxKind.IdentifierNameExpression => null,

            SyntaxKind.I32LiteralExpression => null,
            SyntaxKind.U32LiteralExpression => null,
            SyntaxKind.I64LiteralExpression => null,
            SyntaxKind.U64LiteralExpression => null,
            SyntaxKind.F32LiteralExpression => null,
            SyntaxKind.F64LiteralExpression => null,
            SyntaxKind.StrLiteralExpression => null,
            SyntaxKind.TrueLiteralExpression => null,
            SyntaxKind.FalseLiteralExpression => null,
            SyntaxKind.NullLiteralExpression => null,

            SyntaxKind.GroupExpression => null,
            SyntaxKind.SimpleAssignmentExpression => null,
            SyntaxKind.AddAssignmentExpression => null,
            SyntaxKind.SubtractAssignmentExpression => null,
            SyntaxKind.MultiplyAssignmentExpression => null,
            SyntaxKind.DivideAssignmentExpression => null,
            SyntaxKind.ModuloAssignmentExpression => null,
            SyntaxKind.PowerAssignmentExpression => null,
            SyntaxKind.AndAssignmentExpression => null,
            SyntaxKind.ExclusiveOrAssignmentExpression => null,
            SyntaxKind.OrAssignmentExpression => null,
            SyntaxKind.LeftShiftAssignmentExpression => null,
            SyntaxKind.RightShiftAssignmentExpression => null,
            SyntaxKind.CoalesceAssignmentExpression => null,

            SyntaxKind.VariableDeclaration => null,
            SyntaxKind.FunctionDeclaration => null,
            SyntaxKind.StructDeclaration => null,
            SyntaxKind.PropertyDeclaration => null,
            SyntaxKind.MethodDeclaration => null,
            SyntaxKind.OperatorDeclaration => null,
            SyntaxKind.ConversionDeclaration => null,
            SyntaxKind.LocalDeclaration => null,

            SyntaxKind.EmptyExpression => null,
            SyntaxKind.StatementExpression => null,
            SyntaxKind.BlockExpression => null,
            SyntaxKind.ArrayExpression => null,
            SyntaxKind.StructInitExpression => null,
            SyntaxKind.PropertyInitExpression => null,

            SyntaxKind.ConversionExpression => null,

            SyntaxKind.IndexExpression => null,
            SyntaxKind.InvocationExpression => null,

            SyntaxKind.UnaryPlusExpression => null,
            SyntaxKind.UnaryMinusExpression => null,
            SyntaxKind.OnesComplementExpression => null,
            SyntaxKind.NotExpression => null,

            SyntaxKind.AddExpression => null,
            SyntaxKind.SubtractExpression => null,
            SyntaxKind.MultiplyExpression => null,
            SyntaxKind.DivideExpression => null,
            SyntaxKind.ModuloExpression => null,
            SyntaxKind.PowerExpression => null,
            SyntaxKind.LeftShiftExpression => null,
            SyntaxKind.RightShiftExpression => null,
            SyntaxKind.LogicalOrExpression => null,
            SyntaxKind.LogicalAndExpression => null,
            SyntaxKind.BitwiseOrExpression => null,
            SyntaxKind.BitwiseAndExpression => null,
            SyntaxKind.ExclusiveOrExpression => null,
            SyntaxKind.EqualsExpression => null,
            SyntaxKind.NotEqualsExpression => null,
            SyntaxKind.LessThanExpression => null,
            SyntaxKind.LessThanOrEqualExpression => null,
            SyntaxKind.GreaterThanExpression => null,
            SyntaxKind.GreaterThanOrEqualExpression => null,
            SyntaxKind.CoalesceExpression => null,

            SyntaxKind.UnaryPlusOperator => null,
            SyntaxKind.UnaryMinusOperator => null,
            SyntaxKind.OnesComplementOperator => null,
            SyntaxKind.NotOperator => null,

            SyntaxKind.AddOperator => null,
            SyntaxKind.SubtractOperator => null,
            SyntaxKind.MultiplyOperator => null,
            SyntaxKind.DivideOperator => null,
            SyntaxKind.ModuloOperator => null,
            SyntaxKind.PowerOperator => null,
            SyntaxKind.LeftShiftOperator => null,
            SyntaxKind.RightShiftOperator => null,
            SyntaxKind.LogicalOrOperator => null,
            SyntaxKind.LogicalAndOperator => null,
            SyntaxKind.BitwiseOrOperator => null,
            SyntaxKind.BitwiseAndOperator => null,
            SyntaxKind.ExclusiveOrOperator => null,
            SyntaxKind.EqualsOperator => null,
            SyntaxKind.NotEqualsOperator => null,
            SyntaxKind.LessThanOperator => null,
            SyntaxKind.LessThanOrEqualOperator => null,
            SyntaxKind.GreaterThanOperator => null,
            SyntaxKind.GreaterThanOrEqualOperator => null,
            SyntaxKind.CoalesceOperator => null,

            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)}: '{syntaxKind}'")
        };
    }

    public static SyntaxKind GetKeywordKind(ReadOnlySpan<char> syntaxText)
    {
        return syntaxText switch
        {
            "as" => SyntaxKind.AsKeyword,
            "if" => SyntaxKind.IfKeyword,
            "implicit" => SyntaxKind.ImplicitKeyword,
            "else" => SyntaxKind.ElseKeyword,
            "explicit" => SyntaxKind.ExplicitKeyword,
            "while" => SyntaxKind.WhileKeyword,
            "for" => SyntaxKind.ForKeyword,
            "continue" => SyntaxKind.ContinueKeyword,
            "break" => SyntaxKind.BreakKeyword,
            "return" => SyntaxKind.ReturnKeyword,

            "any" => SyntaxKind.AnyKeyword,
            "unknown" => SyntaxKind.UnknownKeyword,
            "never" => SyntaxKind.NeverKeyword,
            "unit" => SyntaxKind.UnitKeyword,
            "type" => SyntaxKind.TypeKeyword,
            "str" => SyntaxKind.StrKeyword,
            "bool" => SyntaxKind.BoolKeyword,
            "i8" => SyntaxKind.I8Keyword,
            "i16" => SyntaxKind.I16Keyword,
            "i32" => SyntaxKind.I32Keyword,
            "i64" => SyntaxKind.I64Keyword,
            "i128" => SyntaxKind.I128Keyword,
            "isize" => SyntaxKind.ISizeKeyword,
            "u8" => SyntaxKind.U8Keyword,
            "u16" => SyntaxKind.U16Keyword,
            "u32" => SyntaxKind.U32Keyword,
            "u64" => SyntaxKind.U64Keyword,
            "u128" => SyntaxKind.U128Keyword,
            "usize" => SyntaxKind.USizeKeyword,
            "f16" => SyntaxKind.F16Keyword,
            "f32" => SyntaxKind.F32Keyword,
            "f64" => SyntaxKind.F64Keyword,
            "f80" => SyntaxKind.F80Keyword,
            "f128" => SyntaxKind.F128Keyword,

            "struct" => SyntaxKind.StructKeyword,

            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            "null" => SyntaxKind.NullKeyword,

            _ => SyntaxKind.IdentifierToken,
        };
    }

    public static bool IsOperator(SyntaxKind syntaxKind) => syntaxKind
        is SyntaxKind.PlusToken
        or SyntaxKind.MinusToken
        or SyntaxKind.TildeToken
        or SyntaxKind.BangToken
        or SyntaxKind.StarToken
        or SyntaxKind.SlashToken
        or SyntaxKind.PercentToken
        or SyntaxKind.StarStarToken
        or SyntaxKind.LessThanLessThanToken
        or SyntaxKind.GreaterThanGreaterThanToken
        or SyntaxKind.PipeToken
        or SyntaxKind.AmpersandToken
        or SyntaxKind.EqualsEqualsToken
        or SyntaxKind.BangEqualsToken
        or SyntaxKind.LessThanToken
        or SyntaxKind.LessThanEqualsToken
        or SyntaxKind.GreaterThanToken
        or SyntaxKind.GreaterThanEqualsToken;

    public static bool IsAssignmentOperator(SyntaxKind syntaxKind) => syntaxKind
        is SyntaxKind.EqualsToken
        or SyntaxKind.PlusEqualsToken
        or SyntaxKind.MinusEqualsToken
        or SyntaxKind.StarEqualsToken
        or SyntaxKind.SlashEqualsToken
        or SyntaxKind.PercentEqualsToken
        or SyntaxKind.StarStarEqualsToken
        or SyntaxKind.LessThanLessThanEqualsToken
        or SyntaxKind.GreaterThanGreaterThanEqualsToken
        or SyntaxKind.AmpersandEqualsToken
        or SyntaxKind.PipeEqualsToken
        or SyntaxKind.HatEqualsToken
        or SyntaxKind.HookHookEqualsToken;

    public static bool IsKeyword(SyntaxKind syntaxKind) =>
        syntaxKind is >= SyntaxKind.IfKeyword and <= SyntaxKind.NullKeyword;

    public static bool IsPredefinedType(SyntaxKind syntaxKind) =>
        syntaxKind is >= SyntaxKind.AnyKeyword and <= SyntaxKind.F128Keyword;

    public static bool IsNumberLiteralToken(SyntaxKind syntaxKind) =>
        syntaxKind is >= SyntaxKind.I32LiteralToken and <= SyntaxKind.F64LiteralToken;

    public static (SyntaxKind Operator, int Precedence) GetUnaryOperatorPrecedence(SyntaxKind syntaxKind) => syntaxKind switch
    {
        SyntaxKind.BangToken => (SyntaxKind.NotOperator, 8),
        SyntaxKind.MinusToken => (SyntaxKind.UnaryMinusOperator, 8),
        SyntaxKind.PlusToken => (SyntaxKind.UnaryPlusOperator, 8),
        SyntaxKind.TildeToken => (SyntaxKind.OnesComplementOperator, 8),
        _ => (0, 0),
    };

    public static SyntaxKind GetUnaryOperatorExpression(SyntaxKind operatorKind) => operatorKind switch
    {
        SyntaxKind.NotOperator => SyntaxKind.NotExpression,
        SyntaxKind.UnaryMinusOperator => SyntaxKind.UnaryMinusExpression,
        SyntaxKind.UnaryPlusOperator => SyntaxKind.UnaryPlusExpression,
        SyntaxKind.OnesComplementOperator => SyntaxKind.OnesComplementExpression,
        _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)}: '{operatorKind}'")
    };

    public static (SyntaxKind Operator, int Precedence) GetBinaryOperatorPrecedence(SyntaxKind syntaxKind) => syntaxKind switch
    {
        //SyntaxKind.BracketOpenToken or
        //SyntaxKind.ParenthesisOpenToken => 10,
        //SyntaxKind.DotToken => 9,
        //SyntaxKind.DotDotToken => 8,

        SyntaxKind.StarStarToken => (SyntaxKind.PowerOperator, 7),
        SyntaxKind.PercentToken => (SyntaxKind.ModuloOperator, 6),
        SyntaxKind.StarToken => (SyntaxKind.MultiplyOperator, 6),
        SyntaxKind.SlashToken => (SyntaxKind.DivideOperator, 6),
        SyntaxKind.PlusToken => (SyntaxKind.AddOperator, 5),
        SyntaxKind.MinusToken => (SyntaxKind.SubtractOperator, 5),
        SyntaxKind.LessThanLessThanToken => (SyntaxKind.LeftShiftOperator, 4),
        SyntaxKind.GreaterThanGreaterThanToken => (SyntaxKind.RightShiftOperator, 4),
        SyntaxKind.EqualsEqualsToken => (SyntaxKind.EqualsOperator, 3),
        SyntaxKind.BangEqualsToken => (SyntaxKind.NotEqualsOperator, 3),
        SyntaxKind.LessThanToken => (SyntaxKind.LessThanOperator, 3),
        SyntaxKind.LessThanEqualsToken => (SyntaxKind.LessThanOrEqualOperator, 3),
        SyntaxKind.GreaterThanToken => (SyntaxKind.GreaterThanOperator, 3),
        SyntaxKind.GreaterThanEqualsToken => (SyntaxKind.GreaterThanOrEqualOperator, 3),
        SyntaxKind.AmpersandToken => (SyntaxKind.BitwiseAndOperator, 2),
        SyntaxKind.AmpersandAmpersandToken => (SyntaxKind.LogicalAndOperator, 2),
        SyntaxKind.PipeToken => (SyntaxKind.BitwiseOrOperator, 1),
        SyntaxKind.PipePipeToken => (SyntaxKind.LogicalOrOperator, 1),
        SyntaxKind.HatToken => (SyntaxKind.ExclusiveOrOperator, 1),
        SyntaxKind.HookHookToken => (SyntaxKind.CoalesceOperator, 1),
        _ => (0, 0),
    };

    public static SyntaxKind GetBinaryOperatorExpression(SyntaxKind operatorKind) => operatorKind switch
    {
        SyntaxKind.PowerOperator => SyntaxKind.PowerExpression,
        SyntaxKind.ModuloOperator => SyntaxKind.ModuloExpression,
        SyntaxKind.MultiplyOperator => SyntaxKind.MultiplyExpression,
        SyntaxKind.DivideOperator => SyntaxKind.DivideExpression,
        SyntaxKind.AddOperator => SyntaxKind.AddExpression,
        SyntaxKind.SubtractOperator => SyntaxKind.SubtractExpression,
        SyntaxKind.LeftShiftOperator => SyntaxKind.LeftShiftExpression,
        SyntaxKind.RightShiftOperator => SyntaxKind.RightShiftExpression,
        SyntaxKind.EqualsOperator => SyntaxKind.EqualsExpression,
        SyntaxKind.NotEqualsOperator => SyntaxKind.NotEqualsExpression,
        SyntaxKind.LessThanOperator => SyntaxKind.LessThanExpression,
        SyntaxKind.LessThanOrEqualOperator => SyntaxKind.LessThanOrEqualExpression,
        SyntaxKind.GreaterThanOperator => SyntaxKind.GreaterThanExpression,
        SyntaxKind.GreaterThanOrEqualOperator => SyntaxKind.GreaterThanOrEqualExpression,
        SyntaxKind.BitwiseAndOperator => SyntaxKind.BitwiseAndExpression,
        SyntaxKind.LogicalAndOperator => SyntaxKind.LogicalAndExpression,
        SyntaxKind.BitwiseOrOperator => SyntaxKind.BitwiseOrExpression,
        SyntaxKind.LogicalOrOperator => SyntaxKind.LogicalOrExpression,
        SyntaxKind.ExclusiveOrOperator => SyntaxKind.ExclusiveOrExpression,
        SyntaxKind.CoalesceOperator => SyntaxKind.CoalesceExpression,
        _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)}: '{operatorKind}'")
    };

    public static string GetOperatorName(SyntaxKind operatorKind) => operatorKind switch
    {
        SyntaxKind.UnaryPlusOperator => "+",
        SyntaxKind.UnaryMinusOperator => "-",
        SyntaxKind.OnesComplementOperator => "~",
        SyntaxKind.NotOperator => "!",
        SyntaxKind.AddOperator => "+",
        SyntaxKind.SubtractOperator => "-",
        SyntaxKind.MultiplyOperator => "*",
        SyntaxKind.DivideOperator => "/",
        SyntaxKind.ModuloOperator => "%",
        SyntaxKind.PowerOperator => "**",
        SyntaxKind.LeftShiftOperator => "<<",
        SyntaxKind.RightShiftOperator => ">>",
        SyntaxKind.LogicalOrOperator => "||",
        SyntaxKind.LogicalAndOperator => "&&",
        SyntaxKind.BitwiseOrOperator => "|",
        SyntaxKind.BitwiseAndOperator => "&",
        SyntaxKind.ExclusiveOrOperator => "^",
        SyntaxKind.EqualsOperator => "==",
        SyntaxKind.NotEqualsOperator => "!=",
        SyntaxKind.LessThanOperator => "<",
        SyntaxKind.LessThanOrEqualOperator => "<=",
        SyntaxKind.GreaterThanOperator => ">",
        SyntaxKind.GreaterThanOrEqualOperator => ">=",
        SyntaxKind.CoalesceOperator => "??",
        _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{operatorKind}'"),
    };

    public static SyntaxKind GetExpressionOperator(SyntaxKind expressionKind) => expressionKind switch
    {
        SyntaxKind.AddExpression => SyntaxKind.AddOperator,
        SyntaxKind.AddAssignmentExpression => SyntaxKind.AddOperator,
        SyntaxKind.SubtractExpression => SyntaxKind.SubtractOperator,
        SyntaxKind.SubtractAssignmentExpression => SyntaxKind.SubtractOperator,
        SyntaxKind.MultiplyExpression => SyntaxKind.MultiplyOperator,
        SyntaxKind.MultiplyAssignmentExpression => SyntaxKind.MultiplyOperator,
        SyntaxKind.DivideExpression => SyntaxKind.DivideOperator,
        SyntaxKind.DivideAssignmentExpression => SyntaxKind.DivideOperator,
        SyntaxKind.ModuloExpression => SyntaxKind.ModuloOperator,
        SyntaxKind.ModuloAssignmentExpression => SyntaxKind.ModuloOperator,
        SyntaxKind.PowerExpression => SyntaxKind.PowerOperator,
        SyntaxKind.PowerAssignmentExpression => SyntaxKind.PowerOperator,
        SyntaxKind.LeftShiftExpression => SyntaxKind.LeftShiftOperator,
        SyntaxKind.LeftShiftAssignmentExpression => SyntaxKind.LeftShiftOperator,
        SyntaxKind.RightShiftExpression => SyntaxKind.RightShiftOperator,
        SyntaxKind.RightShiftAssignmentExpression => SyntaxKind.RightShiftOperator,
        SyntaxKind.LogicalOrExpression => SyntaxKind.LogicalOrOperator,
        SyntaxKind.BitwiseOrExpression => SyntaxKind.BitwiseOrOperator,
        SyntaxKind.OrAssignmentExpression => SyntaxKind.BitwiseOrOperator,
        SyntaxKind.LogicalAndExpression => SyntaxKind.LogicalAndOperator,
        SyntaxKind.BitwiseAndExpression => SyntaxKind.BitwiseAndOperator,
        SyntaxKind.AndAssignmentExpression => SyntaxKind.BitwiseAndOperator,
        SyntaxKind.ExclusiveOrExpression => SyntaxKind.ExclusiveOrOperator,
        SyntaxKind.ExclusiveOrAssignmentExpression => SyntaxKind.ExclusiveOrOperator,
        SyntaxKind.CoalesceExpression => SyntaxKind.CoalesceOperator,
        SyntaxKind.CoalesceAssignmentExpression => SyntaxKind.CoalesceOperator,
        _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{expressionKind}'"),
    };
}
