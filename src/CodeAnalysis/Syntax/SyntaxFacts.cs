using System.Diagnostics;

namespace CodeAnalysis.Syntax;

public static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecedence(this TokenKind type) => type switch
    {
        TokenKind.Bang or
        TokenKind.Minus or
        TokenKind.Plus or
        TokenKind.Tilde or
        TokenKind.PlusPlus or
        TokenKind.MinusMinus or
        TokenKind.SizeOf or
        TokenKind.TypeOf => 8,

        _ => 0,
    };

    public static int GetBinaryOperatorPrecedence(this TokenKind type) => type switch
    {
        TokenKind.BracketOpen or
        TokenKind.ParenthesisOpen => 10,

        TokenKind.Dot => 9,

        TokenKind.DotDot => 8,

        TokenKind.StarStar => 7,

        TokenKind.Percent or
        TokenKind.Star or
        TokenKind.Slash => 6,

        TokenKind.Plus or
        TokenKind.Minus => 5,

        TokenKind.LessLess or
        TokenKind.GreaterGreater => 4,

        TokenKind.EqualEqual or
        TokenKind.BangEqual or
        TokenKind.Less or
        TokenKind.LessEqual or
        TokenKind.Greater or
        TokenKind.GreaterEqual => 3,

        TokenKind.Ampersand or
        TokenKind.AmpersandAmpersand => 2,

        TokenKind.Arrow or
        TokenKind.Pipe or
        TokenKind.PipePipe or
        TokenKind.Hat or
        TokenKind.HookHook => 1,

        _ => 0,
    };

    public static TokenKind GetKeywordKind(this ReadOnlySpan<char> text) => text switch
    {
        "mutable" => TokenKind.Mutable,
        "sizeof" => TokenKind.SizeOf,
        "typeof" => TokenKind.TypeOf,

        "if" => TokenKind.If,
        "else" => TokenKind.Else,
        "while" => TokenKind.While,
        "for" => TokenKind.For,
        "break" => TokenKind.Break,
        "continue" => TokenKind.Continue,
        "return" => TokenKind.Return,

        "any" => TokenKind.Type_Any,
        "unknown" => TokenKind.Type_Unknown,
        "bool" => TokenKind.Type_Bool,
        "false" => TokenKind.False,
        "true" => TokenKind.True,
        "never" => TokenKind.Type_Never,
        "unit" => TokenKind.Type_Unit,
        "null" => TokenKind.Null,
        "str" => TokenKind.Type_Str,
        "type" => TokenKind.Type_Type,
        "i8" => TokenKind.Type_I8,
        "i16" => TokenKind.Type_I16,
        "i32" => TokenKind.Type_I32,
        "i64" => TokenKind.Type_I64,
        "i128" => TokenKind.Type_I128,
        "isize" => TokenKind.Type_ISize,
        "u8" => TokenKind.Type_U8,
        "u16" => TokenKind.Type_U16,
        "u32" => TokenKind.Type_U32,
        "u64" => TokenKind.Type_U64,
        "u128" => TokenKind.Type_U128,
        "usize" => TokenKind.Type_USize,
        "f16" => TokenKind.Type_F16,
        "f32" => TokenKind.Type_F32,
        "f64" => TokenKind.Type_F64,
        "f80" => TokenKind.Type_F80,
        "f128" => TokenKind.Type_F128,

        _ => TokenKind.Identifier,
    };

    public static bool IsNumber(this TokenKind kind) => kind is >= TokenKind.I32 and <= TokenKind.F32;

    public static bool IsBoolean(this TokenKind kind) => kind is TokenKind.True or TokenKind.False;

    public static bool IsLiteral(this TokenKind kind) => kind.IsNumber() || kind.IsBoolean() || kind is TokenKind.Str || kind is TokenKind.Null;

    public static bool IsComment(this TokenKind kind) => kind is TokenKind.Comment_SingleLine or TokenKind.Comment_MultiLine;

    public static bool IsTrivia(this TokenKind kind) => kind is TokenKind.Invalid or TokenKind.WhiteSpace or TokenKind.LineBreak || kind.IsComment();

    public static bool IsKeyword(this TokenKind kind) => kind
        is TokenKind.Mutable
        or TokenKind.SizeOf
        or TokenKind.TypeOf
        or TokenKind.If
        or TokenKind.Else
        or TokenKind.While
        or TokenKind.For
        or TokenKind.Break
        or TokenKind.Continue
        or TokenKind.Return
        or TokenKind.Type_Any
        or TokenKind.Type_Unknown
        or TokenKind.Type_Bool
        or TokenKind.False
        or TokenKind.True
        or TokenKind.Type_Never
        or TokenKind.Type_Unit
        or TokenKind.Null
        or TokenKind.Type_Str
        or TokenKind.Type_Type
        or TokenKind.Type_I8
        or TokenKind.Type_I16
        or TokenKind.Type_I32
        or TokenKind.Type_I64
        or TokenKind.Type_I128
        or TokenKind.Type_ISize
        or TokenKind.Type_U8
        or TokenKind.Type_U16
        or TokenKind.Type_U32
        or TokenKind.Type_U64
        or TokenKind.Type_U128
        or TokenKind.Type_USize
        or TokenKind.Type_F16
        or TokenKind.Type_F32
        or TokenKind.Type_F64
        or TokenKind.Type_F80
        or TokenKind.Type_F128;

    public static bool IsPredefinedType(this TokenKind kind) => kind
        is TokenKind.Type_Any
        or TokenKind.Type_Unknown
        or TokenKind.Type_Never
        or TokenKind.Type_Unit
        or TokenKind.Type_Type
        or TokenKind.Type_Str
        or TokenKind.Type_Bool
        or TokenKind.Type_I8
        or TokenKind.Type_I16
        or TokenKind.Type_I32
        or TokenKind.Type_I64
        or TokenKind.Type_I128
        or TokenKind.Type_ISize
        or TokenKind.Type_U8
        or TokenKind.Type_U16
        or TokenKind.Type_U32
        or TokenKind.Type_U64
        or TokenKind.Type_U128
        or TokenKind.Type_USize
        or TokenKind.Type_F16
        or TokenKind.Type_F32
        or TokenKind.Type_F64
        or TokenKind.Type_F80
        or TokenKind.Type_F128;

    public static bool IsOperator(this TokenKind kind) => kind.IsUnaryOperator() || kind.IsBinaryOperator();

    public static bool IsUnaryOperator(this TokenKind kind) => kind
        is TokenKind.Bang
        or TokenKind.Minus
        or TokenKind.Plus
        or TokenKind.Tilde;

    public static bool IsBinaryOperator(this TokenKind kind) => kind
        is TokenKind.Ampersand
        or TokenKind.AmpersandAmpersand
        or TokenKind.Arrow
        or TokenKind.BangEqual
        or TokenKind.BracketOpen
        or TokenKind.EqualEqual
        or TokenKind.Dot
        or TokenKind.DotDot
        or TokenKind.Greater
        or TokenKind.GreaterEqual
        or TokenKind.GreaterGreater
        or TokenKind.HookHook
        or TokenKind.Less
        or TokenKind.LessEqual
        or TokenKind.LessLess
        or TokenKind.Minus
        or TokenKind.ParenthesisOpen
        or TokenKind.Percent
        or TokenKind.Pipe
        or TokenKind.PipePipe
        or TokenKind.Plus
        or TokenKind.Slash
        or TokenKind.Star
        or TokenKind.StarStar;

    private readonly static Lazy<IReadOnlyList<TokenKind>> UnaryOperatorsLazy = new(() => [.. Enum.GetValues<TokenKind>().Where(k => GetUnaryOperatorPrecedence(k) > 0)]);
    public static IEnumerable<TokenKind> UnaryOperators => UnaryOperatorsLazy.Value;

    private readonly static Lazy<IReadOnlyList<TokenKind>> BinaryOperatorsLazy = new(() => [.. Enum.GetValues<TokenKind>().Where(k => GetBinaryOperatorPrecedence(k) > 0)]);
    public static IEnumerable<TokenKind> BinaryOperators => BinaryOperatorsLazy.Value;

    public static TokenKind GetBinaryOperatorOfAssignmentOperator(this TokenKind kind)
    {
        return kind switch
        {
            TokenKind.AmpersandEqual => TokenKind.Ampersand,
            TokenKind.GreaterGreaterEqual => TokenKind.GreaterGreater,
            TokenKind.HatEqual => TokenKind.Hat,
            TokenKind.HookHookEqual => TokenKind.HookHook,
            TokenKind.LessLessEqual => TokenKind.LessLess,
            TokenKind.MinusEqual => TokenKind.Minus,
            TokenKind.PercentEqual => TokenKind.Percent,
            TokenKind.PipeEqual => TokenKind.Pipe,
            TokenKind.PlusEqual => TokenKind.Plus,
            TokenKind.StarStarEqual => TokenKind.StarStar,
            TokenKind.SlashEqual => TokenKind.Slash,
            TokenKind.StarEqual => TokenKind.Star,
            _ => throw new UnreachableException($"Unexpected syntax: '{kind}'"),
        };
    }

    public static string? GetText(this TokenKind kind) => kind switch
    {
        // Invalid
        TokenKind.Invalid => null,

        // Punctuation
        TokenKind.BraceOpen => "{",
        TokenKind.BraceClose => "}",
        TokenKind.ParenthesisOpen => "(",
        TokenKind.ParenthesisClose => ")",
        TokenKind.BracketOpen => "[",
        TokenKind.BracketClose => "]",
        TokenKind.Colon => ":",
        TokenKind.Semicolon => ";",
        TokenKind.Comma => ",",

        // Operator
        TokenKind.Ampersand => "&",
        TokenKind.AmpersandAmpersand => "&&",
        TokenKind.AmpersandEqual => "&=",
        TokenKind.Bang => "!",
        TokenKind.BangEqual => "!=",
        // TokenKind.Call => null,
        TokenKind.Arrow => "->",
        TokenKind.Dot => ".",
        TokenKind.DotDot => "..",
        TokenKind.Equal => "=",
        TokenKind.EqualEqual => "==",
        TokenKind.Greater => ">",
        TokenKind.GreaterEqual => ">=",
        TokenKind.GreaterGreater => ">>",
        TokenKind.GreaterGreaterEqual => ">>=",
        TokenKind.Hat => "^",
        TokenKind.HatEqual => "^=",
        TokenKind.Hook => "?",
        TokenKind.HookHook => "??",
        TokenKind.HookHookEqual => "??=",
        TokenKind.Lambda => "=>",
        TokenKind.Less => "<",
        TokenKind.LessEqual => "<=",
        TokenKind.LessLess => "<<",
        TokenKind.LessLessEqual => "<<=",
        TokenKind.Minus => "-",
        TokenKind.MinusEqual => "-=",
        TokenKind.MinusMinus => "--",
        TokenKind.Percent => "%",
        TokenKind.PercentEqual => "%=",
        TokenKind.Pipe => "|",
        TokenKind.PipeEqual => "|=",
        TokenKind.PipePipe => "||",
        TokenKind.Plus => "+",
        TokenKind.PlusEqual => "+=",
        TokenKind.PlusPlus => "++",
        TokenKind.SizeOf => "sizeof",
        TokenKind.Slash => "/",
        TokenKind.SlashEqual => "/=",
        TokenKind.Star => "*",
        TokenKind.StarEqual => "*=",
        TokenKind.StarStar => "**",
        TokenKind.StarStarEqual => "**=",
        // TokenKind.Subscript => null,
        TokenKind.Tilde => "~",
        TokenKind.TypeOf => "typeof",

        // Control flow
        TokenKind.If => "if",
        TokenKind.Else => "else",
        TokenKind.While => "while",
        TokenKind.For => "for",
        TokenKind.Continue => "continue",
        TokenKind.Break => "break",
        TokenKind.Return => "return",

        // Literals
        TokenKind.I32 => null,
        //TokenKind.I64 => null,
        TokenKind.F32 => null,
        //TokenKind.F64 => null,
        TokenKind.Str => null,
        TokenKind.True => "true",
        TokenKind.False => "false",
        TokenKind.Null => "null",

        // Other Keywords
        TokenKind.Mutable => "mutable",

        // Primitive Types
        TokenKind.Type_Any => "any",
        TokenKind.Type_Unknown => "unknown",
        TokenKind.Type_Never => "never",
        TokenKind.Type_Unit => "unit",
        TokenKind.Type_Type => "type",
        // TokenKind.Type_Option => null,
        // TokenKind.Type_Union => null,
        // TokenKind.Type_Array => null,
        TokenKind.Type_Str => "str",
        // TokenKind.Type_Func => null,
        TokenKind.Type_Bool => "bool",
        TokenKind.Type_I8 => "i8",
        TokenKind.Type_I16 => "i16",
        TokenKind.Type_I32 => "i32",
        TokenKind.Type_I64 => "i64",
        TokenKind.Type_I128 => "i128",
        TokenKind.Type_ISize => "isize",
        TokenKind.Type_U8 => "u8",
        TokenKind.Type_U16 => "u16",
        TokenKind.Type_U32 => "u32",
        TokenKind.Type_U64 => "u64",
        TokenKind.Type_U128 => "u128",
        TokenKind.Type_USize => "usize",
        TokenKind.Type_F16 => "f16",
        TokenKind.Type_F32 => "f32",
        TokenKind.Type_F64 => "f64",
        TokenKind.Type_F80 => "f80",
        TokenKind.Type_F128 => "f128",

        // Identifier
        TokenKind.Identifier => null,

        // Trivia
        TokenKind.LineBreak => null,
        TokenKind.WhiteSpace => null,
        TokenKind.Comment_SingleLine => null,
        TokenKind.Comment_MultiLine => null,
        TokenKind.InvalidText => null,

        // EOF
        TokenKind.Eof => null,
        _ => throw new UnreachableException($"Unexpected syntax: '{kind}'"),
    };

    public static string GetDiagnosticDisplay(this TokenKind kind) => kind.GetText() ?? $"<{kind}>";
}
