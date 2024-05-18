﻿using System.Diagnostics;

namespace CodeAnalysis.Syntax;
public static class SyntaxFacts
{
    public static string? GetText(SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.InvalidSyntax => null,

            SyntaxKind.AmpersandToken => "&",
            SyntaxKind.AmpersandAmpersandToken => "&&",
            SyntaxKind.AmpersandEqualToken => "&=",
            SyntaxKind.ArrowToken => "->",
            SyntaxKind.BangToken => "!",
            SyntaxKind.BangEqualToken => "!=",
            SyntaxKind.BraceOpenToken => "{",
            SyntaxKind.BraceCloseToken => "}",
            SyntaxKind.BracketOpenToken => "[",
            SyntaxKind.BracketCloseToken => "]",
            SyntaxKind.ColonToken => ":",
            SyntaxKind.CommaToken => ",",
            SyntaxKind.DotToken => ".",
            SyntaxKind.DotDotToken => "..",
            SyntaxKind.EqualToken => "=",
            SyntaxKind.EqualEqualToken => "==",
            SyntaxKind.GreaterToken => ">",
            SyntaxKind.GreaterEqualToken => ">=",
            SyntaxKind.GreaterGreaterToken => ">>",
            SyntaxKind.GreaterGreaterEqualToken => ">>=",
            SyntaxKind.HatToken => "^",
            SyntaxKind.HatEqualToken => "^=",
            SyntaxKind.HookToken => "?",
            SyntaxKind.HookHookToken => "??",
            SyntaxKind.HookHookEqualToken => "??=",
            SyntaxKind.LambdaToken => "=>",
            SyntaxKind.LessToken => "<",
            SyntaxKind.LessEqualToken => "<=",
            SyntaxKind.LessLessToken => "<<",
            SyntaxKind.LessLessEqualToken => "<<=",
            SyntaxKind.MinusToken => "-",
            SyntaxKind.MinusEqualToken => "-=",
            SyntaxKind.MinusMinusToken => "--",
            SyntaxKind.ParenthesisOpenToken => "(",
            SyntaxKind.ParenthesisCloseToken => ")",
            SyntaxKind.PercentToken => "%",
            SyntaxKind.PercentEqualToken => "%=",
            SyntaxKind.PipeToken => "|",
            SyntaxKind.PipeEqualToken => "|=",
            SyntaxKind.PipePipeToken => "||",
            SyntaxKind.PlusToken => "+",
            SyntaxKind.PlusEqualToken => "+=",
            SyntaxKind.PlusPlusToken => "++",
            SyntaxKind.SemicolonToken => ";",
            SyntaxKind.SlashToken => "/",
            SyntaxKind.SlashEqualToken => "/=",
            SyntaxKind.StarToken => "*",
            SyntaxKind.StarEqualToken => "*=",
            SyntaxKind.StarStarToken => "**",
            SyntaxKind.StarStarEqualToken => "**=",
            SyntaxKind.TildeToken => "~",

            SyntaxKind.I32LiteralToken => null,
            SyntaxKind.I64LiteralToken => null,
            SyntaxKind.F32LiteralToken => null,
            SyntaxKind.F64LiteralToken => null,
            SyntaxKind.StrLiteralToken => null,
            SyntaxKind.TrueLiteralToken => null,
            SyntaxKind.FalseLiteralToken => null,
            SyntaxKind.NullLiteralToken => null,

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

            SyntaxKind.LineBreakTrivia => null,
            SyntaxKind.WhiteSpaceTrivia => null,
            SyntaxKind.SingleLineCommentTrivia => null,
            SyntaxKind.MultiLineCommentTrivia => null,
            SyntaxKind.InvalidTextTrivia => null,

            SyntaxKind.IdentifierToken => null,

            SyntaxKind.EofToken => null,

            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)}: '{kind}'")
        };
    }

    public static string? GetDisplayText(SyntaxKind kind) => GetText(kind) ?? kind.ToString();

    public static SyntaxKind GetKeywordKind(ReadOnlySpan<char> text)
    {
        return text switch
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

            _ => SyntaxKind.IdentifierToken,
        };
    }
}
