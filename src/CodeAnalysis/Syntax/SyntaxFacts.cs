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
            SyntaxKind.MinusMinusToken => "--",
            SyntaxKind.ParenthesisOpenToken => "(",
            SyntaxKind.ParenthesisCloseToken => ")",
            SyntaxKind.PercentToken => "%",
            SyntaxKind.PercentEqualsToken => "%=",
            SyntaxKind.PipeToken => "|",
            SyntaxKind.PipeEqualsToken => "|=",
            SyntaxKind.PipePipeToken => "||",
            SyntaxKind.PlusToken => "+",
            SyntaxKind.PlusEqualsToken => "+=",
            SyntaxKind.PlusPlusToken => "++",
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

            SyntaxKind.IfKeyword => "if",
            SyntaxKind.ElseKeyword => "else",
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
            SyntaxKind.ParameterList => null,
            SyntaxKind.TypeList => null,

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
            SyntaxKind.LocalDeclaration => null,

            SyntaxKind.EmptyExpression => null,
            SyntaxKind.StatementExpression => null,
            SyntaxKind.BlockExpression => null,

            SyntaxKind.UnaryPlusExpression => null,
            SyntaxKind.UnaryMinusExpression => null,
            SyntaxKind.PrefixIncrementExpression => null,
            SyntaxKind.PrefixDecrementExpression => null,
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

            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)}: '{syntaxKind}'")
        };
    }

    public static string? GetDisplayText(SyntaxKind syntaxKind) => GetText(syntaxKind) ?? syntaxKind.ToString();

    public static SyntaxKind GetKeywordKind(ReadOnlySpan<char> syntaxText)
    {
        return syntaxText switch
        {
            "if" => SyntaxKind.IfKeyword,
            "else" => SyntaxKind.ElseKeyword,
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

            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            "null" => SyntaxKind.NullKeyword,

            _ => SyntaxKind.IdentifierToken,
        };
    }

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
        syntaxKind is >= SyntaxKind.UnknownKeyword and <= SyntaxKind.F128Keyword;

    public static bool IsNumberLiteralToken(SyntaxKind syntaxKind) =>
        syntaxKind is >= SyntaxKind.I32LiteralToken and <= SyntaxKind.F64LiteralToken;

    public static (SyntaxKind Expression, int Precedence) GetUnaryOperatorPrecedence(SyntaxKind syntaxKind) => syntaxKind switch
    {
        SyntaxKind.BangToken => (SyntaxKind.NotExpression, 8),
        SyntaxKind.MinusToken => (SyntaxKind.UnaryMinusExpression, 8),
        SyntaxKind.PlusToken => (SyntaxKind.UnaryPlusExpression, 8),
        SyntaxKind.TildeToken => (SyntaxKind.OnesComplementExpression, 8),
        SyntaxKind.PlusPlusToken => (SyntaxKind.PrefixIncrementExpression, 8),
        SyntaxKind.MinusMinusToken => (SyntaxKind.PrefixDecrementExpression, 8),
        _ => (0, 0),
    };

    public static (SyntaxKind Expression, int Precedence) GetBinaryOperatorPrecedence(SyntaxKind syntaxKind) => syntaxKind switch
    {
        //SyntaxKind.BracketOpenToken or
        //SyntaxKind.ParenthesisOpenToken => 10,

        //SyntaxKind.DotToken => 9,

        //SyntaxKind.DotDotToken => 8,

        SyntaxKind.StarStarToken => (SyntaxKind.PowerExpression, 7),

        SyntaxKind.PercentToken => (SyntaxKind.ModuloExpression, 6),
        SyntaxKind.StarToken => (SyntaxKind.MultiplyExpression, 6),
        SyntaxKind.SlashToken => (SyntaxKind.DivideExpression, 6),

        SyntaxKind.PlusToken => (SyntaxKind.AddExpression, 5),
        SyntaxKind.MinusToken => (SyntaxKind.SubtractExpression, 5),

        SyntaxKind.LessThanLessThanToken => (SyntaxKind.LeftShiftExpression, 4),
        SyntaxKind.GreaterThanGreaterThanToken => (SyntaxKind.RightShiftExpression, 4),

        SyntaxKind.EqualsEqualsToken => (SyntaxKind.EqualsExpression, 3),
        SyntaxKind.BangEqualsToken => (SyntaxKind.NotEqualsExpression, 3),
        SyntaxKind.LessThanToken => (SyntaxKind.LessThanExpression, 3),
        SyntaxKind.LessThanEqualsToken => (SyntaxKind.LessThanOrEqualExpression, 3),
        SyntaxKind.GreaterThanToken => (SyntaxKind.GreaterThanExpression, 3),
        SyntaxKind.GreaterThanEqualsToken => (SyntaxKind.GreaterThanOrEqualExpression, 3),

        SyntaxKind.AmpersandToken => (SyntaxKind.BitwiseAndExpression, 2),
        SyntaxKind.AmpersandAmpersandToken => (SyntaxKind.LogicalAndExpression, 2),

        //SyntaxKind.ArrowToken or
        SyntaxKind.PipeToken => (SyntaxKind.BitwiseOrExpression, 1),
        SyntaxKind.PipePipeToken => (SyntaxKind.LogicalOrExpression, 1),
        SyntaxKind.HatToken => (SyntaxKind.ExclusiveOrExpression, 1),
        SyntaxKind.HookHookToken => (SyntaxKind.CoalesceExpression, 1),
        _ => (0, 0),
    };
}
