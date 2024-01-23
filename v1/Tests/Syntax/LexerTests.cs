namespace CodeAnalysis.Syntax;

public sealed class LexerTests
{
    public readonly record struct TokenInfo(TokenKind Kind, string Text);

    [Fact]
    public void Lexer_Tests_AllTokens()
    {
        var tokenKinds = Enum.GetValues<TokenKind>().ToList();

        var testedTokenKinds = GetTokens().Concat(GetSeparatorTokens()).Select(t => t.Kind);

        var untestedTokenKinds = new SortedSet<TokenKind>(tokenKinds);

        untestedTokenKinds.Remove(TokenKind.SingleLineComment);
        untestedTokenKinds.Remove(TokenKind.InvalidText);
        untestedTokenKinds.Remove(TokenKind.Invalid);
        untestedTokenKinds.Remove(TokenKind.EOF);

        untestedTokenKinds.ExceptWith(testedTokenKinds);

        Assert.Empty(untestedTokenKinds);
    }

    public static IEnumerable<object[]> GetTokensData() => GetTokens().Select(t => new object[] { t.Kind, t.Text });

    [Theory]
    [MemberData(nameof(GetTokensData))]
    public void Lexer_Lexes_Token(TokenKind kind, string text)
    {
        var tokens = SyntaxTree.ParseTokens(text);

        var token = Assert.Single(tokens);
        Assert.Equal(kind, token.TokenKind);
        Assert.Equal(text, token.Text);
    }

    [Theory]
    [MemberData(nameof(GetSeparatorTokensData))]
    public void Lexer_Lexes_Trivia(TokenKind kind, string text)
    {
        var tokens = SyntaxTree.ParseTokens(text, includeEof: true);

        var token = Assert.Single(tokens);
        Assert.Equal(TokenKind.EOF, token.TokenKind);
        Assert.Equal(String.Empty, token.Text);
        var trivia = Assert.Single(token.LeadingTrivia);
        Assert.Equal(kind, trivia.TokenKind);
        Assert.Equal(text, trivia.Text);
    }

    private static IEnumerable<TokenInfo> GetSeparatorTokens()
    {
        return new[]
        {
            new TokenInfo(TokenKind.WhiteSpace, " "),
            new TokenInfo(TokenKind.WhiteSpace, "  "),
            new TokenInfo(TokenKind.WhiteSpace, "\t"),
            new TokenInfo(TokenKind.LineBreak, "\r"),
            new TokenInfo(TokenKind.LineBreak, "\n"),
            new TokenInfo(TokenKind.LineBreak, "\r\n"),
            new TokenInfo(TokenKind.MultiLineComment, "/**/"),
        };
    }

    public static IEnumerable<object[]> GetSeparatorTokensData() => GetSeparatorTokens().Select(x => new object[] { x.Kind, x.Text });

    public static IEnumerable<object[]> GetTokenPairsData() => GetTokenPairs().Select(p => new object[] { p.t1, p.t2 });

    [Theory]
    [MemberData(nameof(GetTokenPairsData))]
    public void Lexer_Lexes_TokenPairs(TokenInfo t1, TokenInfo t2)
    {
        var tokens = SyntaxTree.ParseTokens(t1.Text + t2.Text).ToArray();

        Assert.Equal(2, tokens.Length);
        Assert.Equal(t1.Kind, tokens[0].TokenKind);
        Assert.Equal(t1.Text, tokens[0].Text);
        Assert.Equal(t2.Kind, tokens[1].TokenKind);
        Assert.Equal(t2.Text, tokens[1].Text);
    }

    public static IEnumerable<object[]> GetTokenPairsWithSeparatorData() => GetTokenPairsWithSeparator().Select(p => new object[] { p.t1, p.separator, p.t2 });

    [Theory]
    [MemberData(nameof(GetTokenPairsWithSeparatorData))]
    public void Lexer_Lexes_TokenPairsWithSeparator(TokenInfo t1, TokenInfo separator, TokenInfo t2)
    {
        var tokens = SyntaxTree.ParseTokens(t1.Text + separator.Text + t2.Text).ToArray();

        Assert.Equal(2, tokens.Length);
        Assert.Equal(t1.Kind, tokens[0].TokenKind);
        Assert.Equal(t1.Text, tokens[0].Text);

        var trivia = Assert.Single(tokens[0].TrailingTrivia);
        Assert.Equal(separator.Kind, trivia.TokenKind);
        Assert.Equal(separator.Text, trivia.Text);

        Assert.Equal(t2.Kind, tokens[1].TokenKind);
        Assert.Equal(t2.Text, tokens[1].Text);
    }

    private static IEnumerable<TokenInfo> GetTokens()
    {
        return GetFixedTokens().Concat(GetDynamicTokens());

        static IEnumerable<TokenInfo> GetFixedTokens() => Enum.GetValues<TokenKind>()
            .Select(k => new TokenInfo(k, k.GetText()!))
            .Where(t => t.Text is not null);

        static IEnumerable<TokenInfo> GetDynamicTokens() => new[]
        {
            new TokenInfo(TokenKind.I32, "1"),
            new TokenInfo(TokenKind.I32, "123"),

            new TokenInfo(TokenKind.F32, ".1"),
            new TokenInfo(TokenKind.F32, "1.23"),

            new TokenInfo(TokenKind.Identifier, "a"),
            new TokenInfo(TokenKind.Identifier, "abc"),

            new TokenInfo(TokenKind.String, """
                                            "test"
                                            """),
            new TokenInfo(TokenKind.String, """
                                            "te\"st"
                                            """),
            //new TokenInfo(TokenKind.SingleLineComment, "//"),
        };
    }

    private static bool RequireSeparator(TokenKind k1, TokenKind k2)
    {
        if ((k1 is TokenKind.Identifier || k1.IsKeyword()) && (k2 is TokenKind.Identifier || k2.IsKeyword() || k2.IsNumber()))
            return true;

        if (k1.IsNumber() && k2.IsNumber())
            return true;

        switch ((k1, k2))
        {
            case (TokenKind.Ampersand, TokenKind.Ampersand):
            case (TokenKind.Ampersand, TokenKind.AmpersandAmpersand):
            case (TokenKind.Ampersand, TokenKind.AmpersandEqual):
            case (TokenKind.Ampersand, TokenKind.Arrow):
            case (TokenKind.Ampersand, TokenKind.Equal):
            case (TokenKind.Ampersand, TokenKind.EqualEqual):
                return true;

            case (TokenKind.As, TokenKind.As):
                return true;

            case (TokenKind.Bang, TokenKind.Arrow):
            case (TokenKind.Bang, TokenKind.Equal):
            case (TokenKind.Bang, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Equal, TokenKind.Arrow):
            case (TokenKind.Equal, TokenKind.Equal):
            case (TokenKind.Equal, TokenKind.EqualEqual):
            case (TokenKind.Equal, TokenKind.Greater):
            case (TokenKind.Equal, TokenKind.GreaterEqual):
                return true;

            case (TokenKind.Less, TokenKind.Arrow):
            case (TokenKind.Less, TokenKind.Equal):
            case (TokenKind.Less, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Greater, TokenKind.Arrow):
            case (TokenKind.Greater, TokenKind.Equal):
            case (TokenKind.Greater, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Hat, TokenKind.Arrow):
            case (TokenKind.Hat, TokenKind.Equal):
            case (TokenKind.Hat, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Minus, TokenKind.Arrow):
            case (TokenKind.Minus, TokenKind.Equal):
            case (TokenKind.Minus, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Percent, TokenKind.Arrow):
            case (TokenKind.Percent, TokenKind.Equal):
            case (TokenKind.Percent, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Pipe, TokenKind.Arrow):
            case (TokenKind.Pipe, TokenKind.Equal):
            case (TokenKind.Pipe, TokenKind.EqualEqual):
            case (TokenKind.Pipe, TokenKind.Pipe):
            case (TokenKind.Pipe, TokenKind.PipeEqual):
            case (TokenKind.Pipe, TokenKind.PipePipe):
                return true;

            case (TokenKind.Plus, TokenKind.Arrow):
            case (TokenKind.Plus, TokenKind.Equal):
            case (TokenKind.Plus, TokenKind.EqualEqual):
                return true;

            case (TokenKind.String, TokenKind.String):
                return true;

            case (TokenKind.Slash, TokenKind.Arrow):
            case (TokenKind.Slash, TokenKind.Equal):
            case (TokenKind.Slash, TokenKind.EqualEqual):
            case (TokenKind.Slash, TokenKind.Slash):
            case (TokenKind.Slash, TokenKind.SlashEqual):
            case (TokenKind.Slash, TokenKind.Star):
            case (TokenKind.Slash, TokenKind.StarEqual):
            case (TokenKind.Slash, TokenKind.StarStar):
            case (TokenKind.Slash, TokenKind.StarStarEqual):
            case (TokenKind.Slash, TokenKind.SingleLineComment):
            case (TokenKind.Slash, TokenKind.MultiLineComment):
                return true;

            case (TokenKind.Star, TokenKind.Arrow):
            case (TokenKind.Star, TokenKind.Equal):
            case (TokenKind.Star, TokenKind.EqualEqual):
            case (TokenKind.Star, TokenKind.Star):
            case (TokenKind.Star, TokenKind.StarEqual):
            case (TokenKind.Star, TokenKind.StarStar):
            case (TokenKind.Star, TokenKind.StarStarEqual):
                return true;

            case (TokenKind.StarStar, TokenKind.Arrow):
            case (TokenKind.StarStar, TokenKind.Equal):
            case (TokenKind.StarStar, TokenKind.EqualEqual):
            case (TokenKind.StarStar, TokenKind.Star):
            case (TokenKind.StarStar, TokenKind.StarEqual):
            case (TokenKind.StarStar, TokenKind.StarStar):
            case (TokenKind.StarStar, TokenKind.StarStarEqual):
                return true;
        }

        return false;
    }

    private static IEnumerable<(TokenInfo t1, TokenInfo t2)> GetTokenPairs()
    {
        foreach (var t1 in GetTokens())
            foreach (var t2 in GetTokens())
                if (!RequireSeparator(t1.Kind, t2.Kind))
                    yield return (t1, t2);
    }

    private static IEnumerable<(TokenInfo t1, TokenInfo separator, TokenInfo t2)> GetTokenPairsWithSeparator()
    {
        foreach (var t1 in GetTokens())
            foreach (var t2 in GetTokens())
                if (RequireSeparator(t1.Kind, t2.Kind))
                    foreach (var s in GetSeparatorTokens())
                        if (!RequireSeparator(t1.Kind, s.Kind) && !RequireSeparator(s.Kind, t2.Kind))
                            yield return (t1, s, t2);
    }
}