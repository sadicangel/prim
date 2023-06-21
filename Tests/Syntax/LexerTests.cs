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

        untestedTokenKinds.Remove(TokenKind.Invalid);
        untestedTokenKinds.Remove(TokenKind.EOF);

        untestedTokenKinds.ExceptWith(testedTokenKinds);

        Assert.Empty(untestedTokenKinds);
    }

    public static IEnumerable<object[]> GetTokensData() => GetTokens().Concat(GetSeparatorTokens()).Select(t => new object[] { t.Kind, t.Text });

    [Theory]
    [MemberData(nameof(GetTokensData))]
    public void Lexer_Lexes_Token(TokenKind kind, string text)
    {
        var tokens = SyntaxTree.ParseTokens(text.AsMemory());

        var token = Assert.Single(tokens);
        Assert.Equal(kind, token.Kind);
        Assert.Equal(text, token.Text);
    }

    public static IEnumerable<object[]> GetTokenPairsData() => GetTokenPairs().Select(p => new object[] { p.t1, p.t2 });

    [Theory]
    [MemberData(nameof(GetTokenPairsData))]
    public void Lexer_Lexes_TokenPairs(TokenInfo t1, TokenInfo t2)
    {
        var tokens = SyntaxTree.ParseTokens((t1.Text + t2.Text).AsMemory()).ToArray();

        Assert.Equal(2, tokens.Length);
        Assert.Equal(t1.Kind, tokens[0].Kind);
        Assert.Equal(t1.Text, tokens[0].Text);
        Assert.Equal(t2.Kind, tokens[1].Kind);
        Assert.Equal(t2.Text, tokens[1].Text);
    }

    public static IEnumerable<object[]> GetTokenPairsWithSeparatorData() => GetTokenPairsWithSeparator().Select(p => new object[] { p.t1, p.separator, p.t2 });

    [Theory]
    [MemberData(nameof(GetTokenPairsWithSeparatorData))]
    public void Lexer_Lexes_TokenPairsWithSeparator(TokenInfo t1, TokenInfo separator, TokenInfo t2)
    {
        var tokens = SyntaxTree.ParseTokens((t1.Text + separator.Text + t2.Text).AsMemory()).ToArray();

        Assert.Equal(3, tokens.Length);
        Assert.Equal(t1.Kind, tokens[0].Kind);
        Assert.Equal(t1.Text, tokens[0].Text);
        Assert.Equal(separator.Kind, tokens[1].Kind);
        Assert.Equal(separator.Text, tokens[1].Text);
        Assert.Equal(t2.Kind, tokens[2].Kind);
        Assert.Equal(t2.Text, tokens[2].Text);
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

            new TokenInfo(TokenKind.Identifier, "a"),
            new TokenInfo(TokenKind.Identifier, "abc"),
        };
    }

    private static IEnumerable<TokenInfo> GetSeparatorTokens()
    {
        return new[]
        {
            new TokenInfo(TokenKind.WhiteSpace, " "),
            new TokenInfo(TokenKind.WhiteSpace, "  "),
            new TokenInfo(TokenKind.WhiteSpace, "\r"),
            new TokenInfo(TokenKind.WhiteSpace, "\n"),
            new TokenInfo(TokenKind.WhiteSpace, "\r\n"),
        };
    }

    private static bool RequireSeparator(TokenKind k1, TokenKind k2)
    {
        var k1IsKeyword = k1.IsKeyword();
        var k2IsKeyword = k2.IsKeyword();

        if (k1 is TokenKind.Identifier && k2 is TokenKind.Identifier)
            return true;

        if (k1IsKeyword && k2IsKeyword)
            return true;

        if (k1 is TokenKind.Identifier && k2IsKeyword)
            return true;

        if (k1IsKeyword && k2 is TokenKind.Identifier)
            return true;

        if (k1 is TokenKind.I32 && k2 is TokenKind.I32)
            return true;

        if (k1 is TokenKind.Bang && k2 is TokenKind.Equals or TokenKind.EqualsEquals)
            return true;

        if (k1 is TokenKind.Equals && k2 is TokenKind.Equals or TokenKind.EqualsEquals)
            return true;

        if (k1 is TokenKind.Less && k2 is TokenKind.Equals or TokenKind.EqualsEquals)
            return true;

        if (k1 is TokenKind.Greater && k2 is TokenKind.Equals or TokenKind.EqualsEquals)
            return true;

        if (k1 is TokenKind.Ampersand && k2 is TokenKind.Ampersand or TokenKind.AmpersandAmpersand)
            return true;

        if (k1 is TokenKind.Pipe && k2 is TokenKind.Pipe or TokenKind.PipePipe)
            return true;

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
                        yield return (t1, s, t2);
    }
}