namespace CodeAnalysis.Syntax;

internal static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecendence(this TokenKind type) => type switch
    {
        TokenKind.Plus or
        TokenKind.Minus or
        TokenKind.Bang => 6,

        _ => 0,
    };

    public static int GetBinaryOperatorPrecendence(this TokenKind type) => type switch
    {
        TokenKind.Star or
        TokenKind.Slash => 5,

        TokenKind.Plus or
        TokenKind.Minus => 4,

        TokenKind.EqualsEquals or
        TokenKind.BangEquals or
        TokenKind.Less or
        TokenKind.LessEquals or
        TokenKind.Greater or
        TokenKind.GreaterEquals => 3,

        TokenKind.AmpersandAmpersand => 2,

        TokenKind.PipePipe => 1,

        _ => 0,
    };

    public static TokenKind GetKeywordKind(this string text) => GetKeywordKind(text.AsSpan());

    public static TokenKind GetKeywordKind(this ReadOnlySpan<char> text) => text switch
    {
        "const" => TokenKind.Const,
        "else" => TokenKind.Else,
        "false" => TokenKind.False,
        "if" => TokenKind.If,
        "true" => TokenKind.True,
        "var" => TokenKind.Var,
        "while" => TokenKind.While,
        _ => TokenKind.Identifier,
    };

    public static bool IsKeyword(this TokenKind kind) => kind
        is TokenKind.Const
        or TokenKind.Else
        or TokenKind.False
        or TokenKind.If
        or TokenKind.True
        or TokenKind.Var
        or TokenKind.While;

    public static IEnumerable<TokenKind> GetUnaryOperators() => Enum.GetValues<TokenKind>().Where(k => GetUnaryOperatorPrecendence(k) > 0);

    public static IEnumerable<TokenKind> GetBinaryOperators() => Enum.GetValues<TokenKind>().Where(k => GetBinaryOperatorPrecendence(k) > 0);

    public static string? GetText(this TokenKind kind) => kind switch
    {
        TokenKind.Semicolon => ";",
        TokenKind.Plus => "+",
        TokenKind.Minus => "-",
        TokenKind.Star => "*",
        TokenKind.Slash => "/",
        TokenKind.Bang => "!",
        TokenKind.Equals => "=",
        TokenKind.EqualsEquals => "==",
        TokenKind.BangEquals => "!=",
        TokenKind.Less => "<",
        TokenKind.LessEquals => "<=",
        TokenKind.Greater => ">",
        TokenKind.GreaterEquals => ">=",
        TokenKind.AmpersandAmpersand => "&&",
        TokenKind.PipePipe => "||",
        TokenKind.OpenParenthesis => "(",
        TokenKind.CloseParenthesis => ")",
        TokenKind.OpenBrace => "{",
        TokenKind.CloseBrace => "}",
        TokenKind.False => "false",
        TokenKind.True => "true",
        TokenKind.Const => "const",
        TokenKind.Var => "var",
        TokenKind.If => "if",
        TokenKind.Else => "else",
        TokenKind.While => "while",
        _ => null
    };

    public static string GetDiagnosticDisplay(this TokenKind kind) => kind.GetText() ?? $"<{kind}>";
}
