namespace CodeAnalysis.Tests.Scanning;

public partial class LexerTests
{
    private static readonly TokenData[] s_trivia = [
        new TokenData(SyntaxKind.WhiteSpaceTrivia, " "),
        new TokenData(SyntaxKind.WhiteSpaceTrivia, "  "),
        new TokenData(SyntaxKind.WhiteSpaceTrivia, "\t"),
        new TokenData(SyntaxKind.LineBreakTrivia, "\r"),
        new TokenData(SyntaxKind.LineBreakTrivia, "\n"),
        new TokenData(SyntaxKind.LineBreakTrivia, "\r\n"),
        new TokenData(SyntaxKind.MultiLineCommentTrivia, "/**/"),
    ];

    public static TheoryData<TokenData> GetAllTriviaInfo() => new(s_literal_tokens);

    [Theory]
    [MemberData(nameof(GetAllTriviaInfo))]
    public void Scan_scans_all_trivia(TokenData data)
    {
        var token = SyntaxTree.Scan(new SourceText(data.Text))[0];
        Assert.Equal(data.SyntaxKind, token.SyntaxKind);
    }
}
