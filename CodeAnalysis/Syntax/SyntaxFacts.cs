namespace CodeAnalysis.Syntax;

internal static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecendence(this TokenKind type) => type switch
    {
        TokenKind.Bang or
        TokenKind.Minus or
        TokenKind.Plus or
        TokenKind.Tilde => 6,

        _ => 0,
    };

    public static int GetBinaryOperatorPrecendence(this TokenKind type) => type switch
    {
        TokenKind.Percent or
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

        TokenKind.Ampersand or
        TokenKind.AmpersandAmpersand => 2,

        TokenKind.Pipe or
        TokenKind.PipePipe or
        TokenKind.Hat => 1,

        _ => 0,
    };

    public static TokenKind GetKeywordKind(this string text) => GetKeywordKind(text.AsSpan());

    public static TokenKind GetKeywordKind(this ReadOnlySpan<char> text) => text switch
    {
        "const" => TokenKind.Const,
        "else" => TokenKind.Else,
        "false" => TokenKind.False,
        "for" => TokenKind.For,
        "if" => TokenKind.If,
        "in" => TokenKind.In,
        "true" => TokenKind.True,
        "var" => TokenKind.Var,
        "while" => TokenKind.While,
        _ => TokenKind.Identifier,
    };

    public static bool IsKeyword(this TokenKind kind) => kind
        is TokenKind.Const
        or TokenKind.Else
        or TokenKind.False
        or TokenKind.For
        or TokenKind.If
        or TokenKind.In
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
        or TokenKind.BangEquals
        or TokenKind.EqualsEquals
        or TokenKind.Greater
        or TokenKind.GreaterEquals
        or TokenKind.Less
        or TokenKind.LessEquals
        or TokenKind.Minus
        or TokenKind.Percent
        or TokenKind.Pipe
        or TokenKind.PipePipe
        or TokenKind.Plus
        or TokenKind.Slash
        or TokenKind.Star;

    public static IEnumerable<TokenKind> GetUnaryOperators() => Enum.GetValues<TokenKind>().Where(k => GetUnaryOperatorPrecendence(k) > 0);

    public static IEnumerable<TokenKind> GetBinaryOperators() => Enum.GetValues<TokenKind>().Where(k => GetBinaryOperatorPrecendence(k) > 0);

    public static string? GetText(this TokenKind kind) => kind switch
    {
        TokenKind.Ampersand => "&",
        TokenKind.AmpersandAmpersand => "&&",
        TokenKind.Bang => "!",
        TokenKind.BangEquals => "!=",
        TokenKind.CloseBrace => "}",
        TokenKind.CloseParenthesis => ")",
        TokenKind.Const => "const",
        TokenKind.Else => "else",
        TokenKind.Equals => "=",
        TokenKind.EqualsEquals => "==",
        TokenKind.False => "false",
        TokenKind.For => "for",
        TokenKind.Greater => ">",
        TokenKind.GreaterEquals => ">=",
        TokenKind.Hat => "^",
        TokenKind.If => "if",
        TokenKind.In => "in",
        TokenKind.Less => "<",
        TokenKind.LessEquals => "<=",
        TokenKind.Minus => "-",
        TokenKind.OpenBrace => "{",
        TokenKind.OpenParenthesis => "(",
        TokenKind.Percent => "%",
        TokenKind.Pipe => "|",
        TokenKind.PipePipe => "||",
        TokenKind.Plus => "+",
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
