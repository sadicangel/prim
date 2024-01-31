using CodeAnalysis.Syntax;

namespace CodeAnalysis.Tests.Syntax;

public sealed class SyntaxTree_Scan_should
{
    [Fact]
    public void Test_all_tokens()
    {
        var tokenKinds = Enum.GetValues<TokenKind>().ToList();

        var testedTokenKinds = Data.Tokens.Concat(Data.SeparatorTokens).Select(t => t.Kind);

        var untestedTokenKinds = new SortedSet<TokenKind>(tokenKinds);

        untestedTokenKinds.Remove(TokenKind.Comment_SingleLine);
        untestedTokenKinds.Remove(TokenKind.InvalidText);
        untestedTokenKinds.Remove(TokenKind.Invalid);
        untestedTokenKinds.Remove(TokenKind.Eof);

        untestedTokenKinds.ExceptWith(testedTokenKinds);

        Assert.Empty(untestedTokenKinds);
    }

    [Theory]
    [MemberData(nameof(Data.GetTokensData), MemberType = typeof(Data))]
    public void Scan_token(TokenKind kind, string text)
    {
        var tokens = SyntaxTree.Scan(text).SkipLast(1);

        var token = Assert.Single(tokens);
        Assert.Equal(kind, token.TokenKind);
        Assert.Equal(text, token.Text);
    }

    [Theory]
    [MemberData(nameof(Data.GetSeparatorTokensData), MemberType = typeof(Data))]
    public void Scan_trivia(TokenKind kind, string text)
    {
        var tokens = SyntaxTree.Scan(text);

        var token = Assert.Single(tokens);
        Assert.Equal(TokenKind.Eof, token.TokenKind);
        Assert.Equal(String.Empty, token.Text);
        var trivia = Assert.Single(token.Trivia.Leading);
        Assert.Equal(kind, trivia.TokenKind);
        Assert.Equal(text, trivia.Text);
    }

    [Theory]
    [MemberData(nameof(Data.GetTokenPairsData), MemberType = typeof(Data))]
    public void Scan_tokens_in_pairs(TokenInfo t1, TokenInfo t2)
    {
        var tokens = SyntaxTree.Scan(t1.Text + t2.Text).SkipLast(1).ToArray();

        Assert.Equal(2, tokens.Length);
        Assert.Equal(t1.Kind, tokens[0].TokenKind);
        Assert.Equal(t1.Text, tokens[0].Text);
        Assert.Equal(t2.Kind, tokens[1].TokenKind);
        Assert.Equal(t2.Text, tokens[1].Text);
    }


    [Theory]
    [MemberData(nameof(Data.GetTokenPairsWithSeparatorData), MemberType = typeof(Data))]
    public void Scan_tokens_in_pairs_with_separator(TokenInfo t1, TokenInfo separator, TokenInfo t2)
    {
        var tokens = SyntaxTree.Scan(t1.Text + separator.Text + t2.Text).SkipLast(1).ToArray();

        Assert.Equal(2, tokens.Length);
        Assert.Equal(t1.Kind, tokens[0].TokenKind);
        Assert.Equal(t1.Text, tokens[0].Text);

        var trivia = Assert.Single(tokens[0].Trivia.Trailing);
        Assert.Equal(separator.Kind, trivia.TokenKind);
        Assert.Equal(separator.Text, trivia.Text);

        Assert.Equal(t2.Kind, tokens[1].TokenKind);
        Assert.Equal(t2.Text, tokens[1].Text);
    }
}
public readonly record struct TokenInfo(TokenKind Kind, string Text);

file static class Data
{
    public static TheoryData<TokenKind, string> GetTokensData() => Tokens.ToTheoryData(t => (t.Kind, t.Text));
    public static TheoryData<TokenKind, string> GetSeparatorTokensData() => SeparatorTokens.ToTheoryData(t => (t.Kind, t.Text));
    public static TheoryData<TokenInfo, TokenInfo> GetTokenPairsData() => TokenPairs.ToTheoryData(p => (p.t1, p.t2));
    public static TheoryData<TokenInfo, TokenInfo, TokenInfo> GetTokenPairsWithSeparatorData() => TokenPairsWithSeparator.ToTheoryData(p => (p.t1, p.separator, p.t2));

    public static readonly IReadOnlyList<TokenInfo> Tokens = ((Func<IReadOnlyList<TokenInfo>>)(() =>
    {
        return [.. GetFixedTokens().Concat(GetDynamicTokens())];

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

            new TokenInfo(TokenKind.Str, """
                                            "test"
                                            """),
            new TokenInfo(TokenKind.Str, """
                                            "te\"st"
                                            """),
            //new TokenInfo(TokenKind.SingleLineComment, "//"),
        };
    }))();

    public static readonly IReadOnlyList<TokenInfo> SeparatorTokens = ((Func<IReadOnlyList<TokenInfo>>)(() => [
        new TokenInfo(TokenKind.WhiteSpace, " "),
        new TokenInfo(TokenKind.WhiteSpace, "  "),
        new TokenInfo(TokenKind.WhiteSpace, "\t"),
        new TokenInfo(TokenKind.LineBreak, "\r"),
        new TokenInfo(TokenKind.LineBreak, "\n"),
        new TokenInfo(TokenKind.LineBreak, "\r\n"),
        new TokenInfo(TokenKind.Comment_MultiLine, "/**/"),
    ]))();

    public static readonly IReadOnlyList<(TokenInfo t1, TokenInfo t2)> TokenPairs = ((Func<IReadOnlyList<(TokenInfo t1, TokenInfo t2)>>)(() =>
    {
        return [.. EnumerateTokenPairs()];

        static IEnumerable<(TokenInfo t1, TokenInfo t2)> EnumerateTokenPairs()
        {
            foreach (var t1 in Tokens)
                foreach (var t2 in Tokens)
                    if (!RequireSeparator(t1.Kind, t2.Kind))
                        yield return (t1, t2);
        }
    }))();
    public static readonly IReadOnlyList<(TokenInfo t1, TokenInfo separator, TokenInfo t2)> TokenPairsWithSeparator = ((Func<IReadOnlyList<(TokenInfo t1, TokenInfo separator, TokenInfo t2)>>)(() =>
    {
        return [.. EnumerateTokenPairsWithSeparator()];

        static IEnumerable<(TokenInfo t1, TokenInfo separator, TokenInfo t2)> EnumerateTokenPairsWithSeparator()
        {
            foreach (var t1 in Tokens)
                foreach (var t2 in Tokens)
                    if (RequireSeparator(t1.Kind, t2.Kind))
                        foreach (var s in SeparatorTokens)
                            if (!RequireSeparator(t1.Kind, s.Kind) && !RequireSeparator(s.Kind, t2.Kind))
                                yield return (t1, s, t2);
        }
    }))();

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
            case (TokenKind.Ampersand, TokenKind.Lambda):
            case (TokenKind.Ampersand, TokenKind.Equal):
            case (TokenKind.Ampersand, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Arrow, TokenKind.Arrow):
                return true;

            case (TokenKind.Bang, TokenKind.Lambda):
            case (TokenKind.Bang, TokenKind.Equal):
            case (TokenKind.Bang, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Dot, TokenKind.Dot):
            case (TokenKind.Dot, TokenKind.DotDot):
            case (TokenKind.Dot, var kind) when kind.IsNumber():
            case (TokenKind.DotDot, TokenKind.Dot):
                return true;

            case (TokenKind.Equal, TokenKind.Lambda):
            case (TokenKind.Equal, TokenKind.Equal):
            case (TokenKind.Equal, TokenKind.EqualEqual):
            case (TokenKind.Equal, TokenKind.Greater):
            case (TokenKind.Equal, TokenKind.GreaterEqual):
            case (TokenKind.Equal, TokenKind.GreaterGreater):
            case (TokenKind.Equal, TokenKind.GreaterGreaterEqual):
                return true;

            case (TokenKind.Less, TokenKind.Lambda):
            case (TokenKind.Less, TokenKind.Less):
            case (TokenKind.Less, TokenKind.LessEqual):
            case (TokenKind.Less, TokenKind.LessLess):
            case (TokenKind.Less, TokenKind.LessLessEqual):
            case (TokenKind.Less, TokenKind.Equal):
            case (TokenKind.Less, TokenKind.EqualEqual):
            case (TokenKind.LessLess, TokenKind.Lambda):
            case (TokenKind.LessLess, TokenKind.Equal):
            case (TokenKind.LessLess, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Greater, TokenKind.Lambda):
            case (TokenKind.Greater, TokenKind.Greater):
            case (TokenKind.Greater, TokenKind.GreaterEqual):
            case (TokenKind.Greater, TokenKind.GreaterGreater):
            case (TokenKind.Greater, TokenKind.GreaterGreaterEqual):
            case (TokenKind.Greater, TokenKind.Equal):
            case (TokenKind.Greater, TokenKind.EqualEqual):
            case (TokenKind.GreaterGreater, TokenKind.Lambda):
            case (TokenKind.GreaterGreater, TokenKind.Equal):
            case (TokenKind.GreaterGreater, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Hat, TokenKind.Lambda):
            case (TokenKind.Hat, TokenKind.Equal):
            case (TokenKind.Hat, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Hook, TokenKind.Hook):
            case (TokenKind.Hook, TokenKind.HookHook):
            case (TokenKind.Hook, TokenKind.HookHookEqual):
            case (TokenKind.HookHook, TokenKind.Lambda):
            case (TokenKind.HookHook, TokenKind.Equal):
            case (TokenKind.HookHook, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Minus, TokenKind.Arrow):
            case (TokenKind.Minus, TokenKind.Lambda):
            case (TokenKind.Minus, TokenKind.Minus):
            case (TokenKind.Minus, TokenKind.MinusEqual):
            case (TokenKind.Minus, TokenKind.MinusMinus):
            case (TokenKind.Minus, TokenKind.Equal):
            case (TokenKind.Minus, TokenKind.EqualEqual):
            case (TokenKind.Minus, TokenKind.Greater):
            case (TokenKind.Minus, TokenKind.GreaterEqual):
            case (TokenKind.Minus, TokenKind.GreaterGreater):
            case (TokenKind.Minus, TokenKind.GreaterGreaterEqual):
                return true;

            case (TokenKind.Percent, TokenKind.Lambda):
            case (TokenKind.Percent, TokenKind.Equal):
            case (TokenKind.Percent, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Pipe, TokenKind.Lambda):
            case (TokenKind.Pipe, TokenKind.Equal):
            case (TokenKind.Pipe, TokenKind.EqualEqual):
            case (TokenKind.Pipe, TokenKind.Pipe):
            case (TokenKind.Pipe, TokenKind.PipeEqual):
            case (TokenKind.Pipe, TokenKind.PipePipe):
                return true;

            case (TokenKind.Plus, TokenKind.Lambda):
            case (TokenKind.Plus, TokenKind.Plus):
            case (TokenKind.Plus, TokenKind.PlusEqual):
            case (TokenKind.Plus, TokenKind.PlusPlus):
            case (TokenKind.Plus, TokenKind.Equal):
            case (TokenKind.Plus, TokenKind.EqualEqual):
                return true;

            case (TokenKind.Str, TokenKind.Str):
                return true;

            case (TokenKind.Slash, TokenKind.Lambda):
            case (TokenKind.Slash, TokenKind.Equal):
            case (TokenKind.Slash, TokenKind.EqualEqual):
            case (TokenKind.Slash, TokenKind.Slash):
            case (TokenKind.Slash, TokenKind.SlashEqual):
            case (TokenKind.Slash, TokenKind.Star):
            case (TokenKind.Slash, TokenKind.StarEqual):
            case (TokenKind.Slash, TokenKind.StarStar):
            case (TokenKind.Slash, TokenKind.StarStarEqual):
            case (TokenKind.Slash, TokenKind.Comment_SingleLine):
            case (TokenKind.Slash, TokenKind.Comment_MultiLine):
                return true;

            case (TokenKind.Star, TokenKind.Lambda):
            case (TokenKind.Star, TokenKind.Equal):
            case (TokenKind.Star, TokenKind.EqualEqual):
            case (TokenKind.Star, TokenKind.Star):
            case (TokenKind.Star, TokenKind.StarEqual):
            case (TokenKind.Star, TokenKind.StarStar):
            case (TokenKind.Star, TokenKind.StarStarEqual):
                return true;

            case (TokenKind.StarStar, TokenKind.Lambda):
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
}