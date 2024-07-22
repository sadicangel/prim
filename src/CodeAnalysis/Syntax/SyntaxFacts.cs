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
            SyntaxKind.BracketOpenBracketCloseToken => "[]",
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
            SyntaxKind.ParenthesisOpenParenthesisCloseToken => "()",
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

            SyntaxKind.ThisKeyword => "this",

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
            SyntaxKind.LambdaType => null,
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

            SyntaxKind.VariableDeclaration => null,
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

            SyntaxKind.IndexExpression => null,
            SyntaxKind.InvocationExpression => null,
            SyntaxKind.MemberAccessExpression => null,
            SyntaxKind.ConversionExpression => null,

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

            SyntaxKind.AssignmentExpression => null,

            SyntaxKind.IfExpression => null,
            SyntaxKind.ElseClauseExpression => null,
            SyntaxKind.WhileExpression => null,

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

            "this" => SyntaxKind.ThisKeyword,

            _ => SyntaxKind.IdentifierToken,
        };
    }


    public static bool IsOperator(SyntaxKind syntaxKind) => syntaxKind
        is SyntaxKind.AmpersandToken
        or SyntaxKind.AmpersandAmpersandToken
        or SyntaxKind.BangToken
        or SyntaxKind.BangEqualsToken
        or SyntaxKind.BracketOpenBracketCloseToken
        or SyntaxKind.DotDotToken
        or SyntaxKind.EqualsEqualsToken
        or SyntaxKind.MinusGreaterThanToken
        or SyntaxKind.GreaterThanToken
        or SyntaxKind.GreaterThanEqualsToken
        or SyntaxKind.GreaterThanGreaterThanToken
        or SyntaxKind.GreaterThanGreaterThanEqualsToken
        or SyntaxKind.HatToken
        or SyntaxKind.HookHookToken
        or SyntaxKind.LessThanToken
        or SyntaxKind.LessThanEqualsToken
        or SyntaxKind.LessThanLessThanToken
        or SyntaxKind.LessThanLessThanEqualsToken
        or SyntaxKind.MinusToken
        or SyntaxKind.ParenthesisOpenParenthesisCloseToken
        or SyntaxKind.PercentToken
        or SyntaxKind.PipeToken
        or SyntaxKind.PipePipeToken
        or SyntaxKind.PlusToken
        or SyntaxKind.SlashToken
        or SyntaxKind.StarToken
        or SyntaxKind.StarStarToken
        or SyntaxKind.TildeToken;

    public static bool IsKeyword(SyntaxKind syntaxKind) =>
        syntaxKind is >= SyntaxKind.IfKeyword and <= SyntaxKind.NullKeyword;

    public static bool IsPredefinedType(SyntaxKind syntaxKind) =>
        syntaxKind is >= SyntaxKind.AnyKeyword and <= SyntaxKind.F128Keyword;

    public static bool IsNumberLiteralToken(SyntaxKind syntaxKind) =>
        syntaxKind is >= SyntaxKind.I32LiteralToken and <= SyntaxKind.F64LiteralToken;

    public static int GetUnaryOperatorPrecedence(SyntaxKind syntaxKind) => syntaxKind switch
    {
        SyntaxKind.BangToken => 8,
        SyntaxKind.MinusToken => 8,
        SyntaxKind.PlusToken => 8,
        SyntaxKind.TildeToken => 8,
        _ => 0,
    };

    public static SyntaxKind GetUnaryOperatorExpression(SyntaxKind operatorKind) => operatorKind switch
    {
        SyntaxKind.BangToken => SyntaxKind.NotExpression,
        SyntaxKind.MinusToken => SyntaxKind.UnaryMinusExpression,
        SyntaxKind.PlusToken => SyntaxKind.UnaryPlusExpression,
        SyntaxKind.TildeToken => SyntaxKind.OnesComplementExpression,
        _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)}: '{operatorKind}'")
    };

    public static int GetBinaryOperatorPrecedence(SyntaxKind syntaxKind) => syntaxKind switch
    {
        SyntaxKind.StarStarToken => 7,
        SyntaxKind.PercentToken => 6,
        SyntaxKind.StarToken => 6,
        SyntaxKind.SlashToken => 6,
        SyntaxKind.PlusToken => 5,
        SyntaxKind.MinusToken => 5,
        SyntaxKind.LessThanLessThanToken => 4,
        SyntaxKind.GreaterThanGreaterThanToken => 4,
        SyntaxKind.EqualsEqualsToken => 3,
        SyntaxKind.BangEqualsToken => 3,
        SyntaxKind.LessThanToken => 3,
        SyntaxKind.LessThanEqualsToken => 3,
        SyntaxKind.GreaterThanToken => 3,
        SyntaxKind.GreaterThanEqualsToken => 3,
        SyntaxKind.AmpersandToken => 2,
        SyntaxKind.AmpersandAmpersandToken => 2,
        SyntaxKind.PipeToken => 1,
        SyntaxKind.PipePipeToken => 1,
        SyntaxKind.HatToken => 1,
        SyntaxKind.HookHookToken => 1,
        _ => 0,
    };

    public static SyntaxKind GetBinaryOperatorExpression(SyntaxKind operatorKind) => operatorKind switch
    {
        SyntaxKind.StarStarToken => SyntaxKind.PowerExpression,
        SyntaxKind.PercentToken => SyntaxKind.ModuloExpression,
        SyntaxKind.StarToken => SyntaxKind.MultiplyExpression,
        SyntaxKind.SlashToken => SyntaxKind.DivideExpression,
        SyntaxKind.PlusToken => SyntaxKind.AddExpression,
        SyntaxKind.MinusToken => SyntaxKind.SubtractExpression,
        SyntaxKind.LessThanLessThanToken => SyntaxKind.LeftShiftExpression,
        SyntaxKind.GreaterThanGreaterThanToken => SyntaxKind.RightShiftExpression,
        SyntaxKind.EqualsEqualsToken => SyntaxKind.EqualsExpression,
        SyntaxKind.BangEqualsToken => SyntaxKind.NotEqualsExpression,
        SyntaxKind.LessThanToken => SyntaxKind.LessThanExpression,
        SyntaxKind.LessThanEqualsToken => SyntaxKind.LessThanOrEqualExpression,
        SyntaxKind.GreaterThanToken => SyntaxKind.GreaterThanExpression,
        SyntaxKind.GreaterThanEqualsToken => SyntaxKind.GreaterThanOrEqualExpression,
        SyntaxKind.AmpersandToken => SyntaxKind.BitwiseAndExpression,
        SyntaxKind.AmpersandAmpersandToken => SyntaxKind.LogicalAndExpression,
        SyntaxKind.PipeToken => SyntaxKind.BitwiseOrExpression,
        SyntaxKind.PipePipeToken => SyntaxKind.LogicalOrExpression,
        SyntaxKind.HatToken => SyntaxKind.ExclusiveOrExpression,
        SyntaxKind.HookHookToken => SyntaxKind.CoalesceExpression,
        _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)}: '{operatorKind}'")
    };
}
