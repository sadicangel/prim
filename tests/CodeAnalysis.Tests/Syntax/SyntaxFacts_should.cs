using CodeAnalysis.Syntax;

namespace CodeAnalysis.Tests.Syntax;
public sealed class SyntaxFacts_should
{
    [Theory]
    [MemberData(nameof(GetTokenKindData))]
    public void Roundtrip_token_text(TokenKind kind)
    {
        var text = kind.GetText();
        if (text is null)
            return;

        var tokens = SyntaxTree.Scan(text).SkipLast(1);
        var token = Assert.Single(tokens);
        Assert.Equal(kind, token.TokenKind);
        Assert.Equal(text, token.Text);
    }

    public static TheoryData<TokenKind> GetTokenKindData() => Enum.GetValues<TokenKind>().ToTheoryData();
}
