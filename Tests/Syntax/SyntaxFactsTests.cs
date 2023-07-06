namespace CodeAnalysis.Syntax;
public sealed class SyntaxFactsTests
{
    [Theory]
    [MemberData(nameof(GetTokenKindData))]
    public void SyntaxFacts_GetText_Roundtrips(TokenKind kind)
    {
        var text = kind.GetText();
        if (text is null)
            return;

        var tokens = SyntaxTree.ParseTokens(text);
        var token = Assert.Single(tokens);
        Assert.Equal(kind, token.TokenKind);
        Assert.Equal(text, token.Text);
    }

    public static IEnumerable<object[]> GetTokenKindData() => Enum.GetValues<TokenKind>().Select(e => new object[] { e });
}
