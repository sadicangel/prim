namespace CodeAnalysis.Syntax;

internal static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecendence(this TokenKind type) => type switch
    {
        TokenKind.Plus or TokenKind.Minus or TokenKind.Bang => 6,
        _ => 0,
    };

    public static int GetBinaryOperatorPrecendence(this TokenKind type) => type switch
    {
        TokenKind.Star or TokenKind.Slash => 5,
        TokenKind.Plus or TokenKind.Minus => 4,
        TokenKind.EqualsEquals or TokenKind.BangEquals => 3,
        TokenKind.AmpersandAmpersand => 2,
        TokenKind.PipePipe => 1,
        _ => 0,
    };

    public static TokenKind GetKeywordKind(this string text) => text switch
    {
        "false" => TokenKind.False,
        "true" => TokenKind.True,
        _ => TokenKind.Identifier,
    };

    public static IEnumerable<TokenKind> GetUnaryOperators() => Enum.GetValues<TokenKind>().Where(k => GetUnaryOperatorPrecendence(k) > 0);

    public static IEnumerable<TokenKind> GetBinaryOperators() => Enum.GetValues<TokenKind>().Where(k => GetBinaryOperatorPrecendence(k) > 0);

    public static string? GetText(this TokenKind kind) => kind switch
    {
        TokenKind.Plus => "+",
        TokenKind.Minus => "-",
        TokenKind.Star => "*",
        TokenKind.Slash => "/",
        TokenKind.Bang => "!",
        TokenKind.Equals => "=",
        TokenKind.AmpersandAmpersand => "&&",
        TokenKind.PipePipe => "||",
        TokenKind.EqualsEquals => "==",
        TokenKind.BangEquals => "!=",
        TokenKind.OpenParenthesis => "(",
        TokenKind.CloseParenthesis => ")",
        TokenKind.False => "false",
        TokenKind.True => "true",
        _ => null
    };
}
