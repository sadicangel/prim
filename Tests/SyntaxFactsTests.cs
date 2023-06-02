using CodeAnalysis.Syntax;

namespace Tests;
public sealed class SyntaxFactsTests
{
    [Theory]
    [MemberData(nameof(GetTokenKindData))]
    public void SyntaxFacts_GetText_Roundtrips(TokenKind kind)
    {
        var text = SyntaxFacts.GetText(kind);
        if (text is null)
            return;

        var tokens = SyntaxTree.ParseTokens(text);
        var token = Assert.Single(tokens);
        Assert.Equal(kind, token.Kind);
        Assert.Equal(text, token.Text);
    }

    public static IEnumerable<object[]> GetTokenKindData() => Enum.GetValues<TokenKind>().Select(e => new object[] { e });
}
