namespace CodeAnalysis.Tests.Parsing;
public partial class ScannerTests
{
    public readonly record struct TokenData(SyntaxKind SyntaxKind, string Text);
    private static readonly TokenData[] s_literal_tokens = [
        new(SyntaxKind.IdentifierToken, "a"),
        new(SyntaxKind.IdentifierToken, "_a"),
        //new(SyntaxKind.I32LiteralToken, "-2147483648"),
        new(SyntaxKind.I32LiteralToken, "42"),
        new(SyntaxKind.I32LiteralToken, "2147483647"),
        new(SyntaxKind.I32LiteralToken, "0x1A3F"),
        new(SyntaxKind.I32LiteralToken, "0b101010"),
        new(SyntaxKind.U32LiteralToken, "42u"),
        new(SyntaxKind.U32LiteralToken, "42U"),
        new(SyntaxKind.U32LiteralToken, "0x1A3Fu"),
        new(SyntaxKind.U32LiteralToken, "0x1A3FU"),
        new(SyntaxKind.U32LiteralToken, "0b101010u"),
        new(SyntaxKind.U32LiteralToken, "0b101010U"),
        //new(SyntaxKind.I64LiteralToken, "-2147483649"),
        new(SyntaxKind.I64LiteralToken, "42l"),
        new(SyntaxKind.I64LiteralToken, "42L"),
        new(SyntaxKind.I64LiteralToken, "0x1A3Fl"),
        new(SyntaxKind.I64LiteralToken, "0x1A3FL"),
        new(SyntaxKind.I64LiteralToken, "0b101010l"),
        new(SyntaxKind.I64LiteralToken, "0b101010L"),
        new(SyntaxKind.I64LiteralToken, "2147483648"),
        new(SyntaxKind.F32LiteralToken, ".2f"),
        new(SyntaxKind.F32LiteralToken, "0.2f"),
        new(SyntaxKind.F32LiteralToken, "4.2f"),
        new(SyntaxKind.F32LiteralToken, ".2e2f"),
        new(SyntaxKind.F32LiteralToken, "0.2e2f"),
        new(SyntaxKind.F32LiteralToken, "4.2e2f"),
        new(SyntaxKind.F32LiteralToken, "42f"),
        new(SyntaxKind.F32LiteralToken, "42F"),
        new(SyntaxKind.F64LiteralToken, ".2"),
        new(SyntaxKind.F64LiteralToken, ".2d"),
        new(SyntaxKind.F64LiteralToken, ".2D"),
        new(SyntaxKind.F64LiteralToken, "0.2"),
        new(SyntaxKind.F64LiteralToken, "0.2d"),
        new(SyntaxKind.F64LiteralToken, "0.2D"),
        new(SyntaxKind.F64LiteralToken, "4.2"),
        new(SyntaxKind.F64LiteralToken, "4.2d"),
        new(SyntaxKind.F64LiteralToken, "4.2D"),
        new(SyntaxKind.F64LiteralToken, ".2e2"),
        new(SyntaxKind.F64LiteralToken, ".2e2d"),
        new(SyntaxKind.F64LiteralToken, ".2e2D"),
        new(SyntaxKind.F64LiteralToken, "0.2e2"),
        new(SyntaxKind.F64LiteralToken, "0.2e2d"),
        new(SyntaxKind.F64LiteralToken, "0.2e2D"),
        new(SyntaxKind.F64LiteralToken, "4.2e2"),
        new(SyntaxKind.F64LiteralToken, "4.2e2d"),
        new(SyntaxKind.F64LiteralToken, "4.2e2D"),
        new(SyntaxKind.F64LiteralToken, "42d"),
        new(SyntaxKind.F64LiteralToken, "42D"),

        //new(SyntaxKind.F32LiteralToken, "0.1"),
        //new(SyntaxKind.F32LiteralToken, ".1"),
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
        Assert.Equal(data.SyntaxKind, token.SyntaxKind);
    }

    public static TheoryData<TokenData> GetAllTokenInfo() => new(Enum.GetValues<SyntaxKind>()
        .Select(sk => new TokenData(sk, SyntaxFacts.GetText(sk)!))
        .Where(td => td.Text is not null)
        .Concat(s_literal_tokens));
}
