namespace CodeAnalysis.Tests.Parsing;
public partial class ScannerTests
{
    public readonly record struct TokenData(SyntaxKind SyntaxKind, string Text);

    private static readonly TokenData[] s_literal_tokens = [
        new(SyntaxKind.IdentifierToken, "a"),
        new(SyntaxKind.IdentifierToken, "_a"),
        new(SyntaxKind.I32LiteralToken, "42"),
        //new(SyntaxKind.I64LiteralToken, "42"),
        new(SyntaxKind.F32LiteralToken, "0.1"),
        new(SyntaxKind.F32LiteralToken, ".1"),
        //new(SyntaxKind.F64LiteralToken, "0.1"),
        //new(SyntaxKind.F64LiteralToken, ".1"),
        new(SyntaxKind.StrLiteralToken, "\"test\""),
        new(SyntaxKind.StrLiteralToken, "\"te\\\"st\""),
    ];

    [Theory]
    [MemberData(nameof(GetAllTokenInfo))]
    public void Scan_scans_all_tokens(TokenData data)
    {
        var token = SyntaxTree.Scan(new SourceText(data.Text))[0];
        Assert.Equal(data.Text, token.Text);
        Assert.Equal(data.SyntaxKind, token.SyntaxKind);
    }

    public static TheoryData<TokenData> GetAllTokenInfo() => new(Enum.GetValues<SyntaxKind>()
        .Select(sk => new TokenData(sk, SyntaxFacts.GetText(sk)!))
        .Where(td => td.Text is not null)
        .Concat(s_literal_tokens));
}
