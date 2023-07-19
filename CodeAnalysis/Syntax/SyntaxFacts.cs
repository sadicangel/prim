namespace CodeAnalysis.Syntax;

public static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecedence(this TokenKind type) => type switch
    {
        TokenKind.Bang or
        TokenKind.Minus or
        TokenKind.Plus or
        TokenKind.Tilde => 6,

        _ => 0,
    };

    public static int GetBinaryOperatorPrecedence(this TokenKind type) => type switch
    {
        TokenKind.Percent or
        TokenKind.Star or
        TokenKind.Slash => 5,

        TokenKind.Plus or
        TokenKind.Minus => 4,

        TokenKind.EqualEqual or
        TokenKind.BangEqual or
        TokenKind.Less or
        TokenKind.LessEqual or
        TokenKind.Greater or
        TokenKind.GreaterEqual => 3,

        TokenKind.Ampersand or
        TokenKind.AmpersandAmpersand => 2,

        TokenKind.As or
        TokenKind.Pipe or
        TokenKind.PipePipe or
        TokenKind.Hat => 1,

        _ => 0,
    };

    public static TokenKind GetKeywordKind(this string text) => GetKeywordKind(text.AsSpan());

    public static TokenKind GetKeywordKind(this ReadOnlySpan<char> text) => text switch
    {
        "as" => TokenKind.As,
        "break" => TokenKind.Break,
        "continue" => TokenKind.Continue,
        "else" => TokenKind.Else,
        "false" => TokenKind.False,
        "for" => TokenKind.For,
        "if" => TokenKind.If,
        "in" => TokenKind.In,
        "let" => TokenKind.Let,
        "return" => TokenKind.Return,
        "true" => TokenKind.True,
        "var" => TokenKind.Var,
        "while" => TokenKind.While,
        _ => TokenKind.Identifier,
    };

    public static bool IsNumber(this TokenKind kind) => kind is TokenKind.I32 or TokenKind.F32;
    public static bool IsBoolean(this TokenKind kind) => kind is TokenKind.True or TokenKind.False;
    public static bool IsLiteral(this TokenKind kind) => kind.IsNumber() || kind.IsBoolean();
    public static bool IsComment(this TokenKind kind) => kind is TokenKind.SingleLineComment or TokenKind.MultiLineComment;

    public static bool IsKeyword(this TokenKind kind) => kind
        is TokenKind.As
        or TokenKind.Break
        or TokenKind.Continue
        or TokenKind.Else
        or TokenKind.False
        or TokenKind.For
        or TokenKind.If
        or TokenKind.In
        or TokenKind.Let
        or TokenKind.Return
        or TokenKind.True
        or TokenKind.Var
        or TokenKind.While;

    public static bool IsOperator(this TokenKind kind) => kind.IsUnaryOperator() || kind.IsBinaryOperator();

    public static bool IsUnaryOperator(this TokenKind kind) => kind
        is TokenKind.Bang
        or TokenKind.Minus
        or TokenKind.Plus
        or TokenKind.Tilde;

    public static bool IsBinaryOperator(this TokenKind kind) => kind
        is TokenKind.Ampersand
        or TokenKind.AmpersandAmpersand
        or TokenKind.BangEqual
        or TokenKind.EqualEqual
        or TokenKind.Greater
        or TokenKind.GreaterEqual
        or TokenKind.Less
        or TokenKind.LessEqual
        or TokenKind.Minus
        or TokenKind.Percent
        or TokenKind.Pipe
        or TokenKind.PipePipe
        or TokenKind.Plus
        or TokenKind.Slash
        or TokenKind.Star;

    public static IEnumerable<TokenKind> GetUnaryOperators() => Enum.GetValues<TokenKind>().Where(k => GetUnaryOperatorPrecedence(k) > 0);

    public static IEnumerable<TokenKind> GetBinaryOperators() => Enum.GetValues<TokenKind>().Where(k => GetBinaryOperatorPrecedence(k) > 0);

    public static string? GetText(this TokenKind kind) => kind switch
    {
        TokenKind.Ampersand => "&",
        TokenKind.AmpersandAmpersand => "&&",
        TokenKind.Arrow => "=>",
        TokenKind.As => "as",
        TokenKind.Bang => "!",
        TokenKind.BangEqual => "!=",
        TokenKind.Break => "break",
        TokenKind.CloseBrace => "}",
        TokenKind.CloseParenthesis => ")",
        TokenKind.Colon => ":",
        TokenKind.Comma => ",",
        TokenKind.Continue => "continue",
        TokenKind.Else => "else",
        TokenKind.Equal => "=",
        TokenKind.EqualEqual => "==",
        TokenKind.False => "false",
        TokenKind.For => "for",
        TokenKind.Greater => ">",
        TokenKind.GreaterEqual => ">=",
        TokenKind.Hat => "^",
        TokenKind.If => "if",
        TokenKind.In => "in",
        TokenKind.Less => "<",
        TokenKind.LessEqual => "<=",
        TokenKind.Let => "let",
        TokenKind.Minus => "-",
        TokenKind.OpenBrace => "{",
        TokenKind.OpenParenthesis => "(",
        TokenKind.Percent => "%",
        TokenKind.Pipe => "|",
        TokenKind.PipePipe => "||",
        TokenKind.Plus => "+",
        TokenKind.Return => "return",
        TokenKind.Semicolon => ";",
        TokenKind.Slash => "/",
        TokenKind.Star => "*",
        TokenKind.Range => "..",
        TokenKind.Tilde => "~",
        TokenKind.True => "true",
        TokenKind.Var => "var",
        TokenKind.While => "while",
        _ => null
    };

    public static string GetDiagnosticDisplay(this TokenKind kind) => kind.GetText() ?? $"<{kind}>";
}
