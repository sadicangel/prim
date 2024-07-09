namespace CodeAnalysis.Tests.Syntax;
public sealed partial class SyntaxFactsTests
{
    public static TheoryData<SyntaxKind> GetAllSyntaxKind() => new(Enum.GetValues<SyntaxKind>().Except([SyntaxKind.BracketOpenBracketCloseToken, SyntaxKind.ParenthesisOpenParenthesisCloseToken]));

    [Theory]
    [MemberData(nameof(GetAllSyntaxKind))]
    public void GetText_does_not_throw(SyntaxKind syntaxKind)
        => Assert.Null(Record.Exception(() => SyntaxFacts.GetText(syntaxKind)));

    [Theory]
    [MemberData(nameof(GetAllSyntaxKind))]
    public void GetText_result_can_be_scanned_into_a_token(SyntaxKind syntaxKind)
    {
        var text = SyntaxFacts.GetText(syntaxKind);
        if (text is null)
            return;

        var token = Assert.Single(SyntaxTree.Scan(new SourceText(text)).SkipLast(1));
        Assert.Equal(syntaxKind, token.SyntaxKind);
        Assert.Equal(text, token.Text);
    }
}
