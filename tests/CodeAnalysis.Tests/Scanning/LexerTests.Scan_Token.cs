namespace CodeAnalysis.Tests.Scanning;
public partial class LexerTests
{
    private static readonly TokenData[] s_literal_tokens = [
        new(SyntaxKind.IdentifierToken, "a"),
        new(SyntaxKind.IdentifierToken, "_a"),
        //new(SyntaxKind.I8LiteralToken, "-42i8"),
        new(SyntaxKind.I8LiteralToken, "42i8"),
        new(SyntaxKind.I16LiteralToken, "42i16"),
        //new(SyntaxKind.I16LiteralToken, "-42i16"),
        new(SyntaxKind.U8LiteralToken, "42u8"),
        new(SyntaxKind.U16LiteralToken, "42u16"),
        //new(SyntaxKind.I32LiteralToken, "-2147483648"),
        new(SyntaxKind.I32LiteralToken, "2147483647"),
        new(SyntaxKind.I32LiteralToken, "2147483647i32"),
        new(SyntaxKind.I32LiteralToken, "0x1A3F"),
        new(SyntaxKind.I32LiteralToken, "0b101010"),
        new(SyntaxKind.U32LiteralToken, "42u32"),
        new(SyntaxKind.U32LiteralToken, "0x1A3Fu32"),
        new(SyntaxKind.U32LiteralToken, "0x1A3Fu32"),
        new(SyntaxKind.U32LiteralToken, "0b101010u32"),
        new(SyntaxKind.U32LiteralToken, "0b101010u32"),
        //new(SyntaxKind.I64LiteralToken, "-2147483649"),
        new(SyntaxKind.I64LiteralToken, "42i64"),
        new(SyntaxKind.I64LiteralToken, "42i64"),
        new(SyntaxKind.I64LiteralToken, "0x1A3Fi64"),
        new(SyntaxKind.I64LiteralToken, "0x1A3Fi64"),
        new(SyntaxKind.I64LiteralToken, "0b101010i64"),
        new(SyntaxKind.I64LiteralToken, "0b101010i64"),
        new(SyntaxKind.I64LiteralToken, "2147483648"),
        new(SyntaxKind.F32LiteralToken, ".2f32"),
        new(SyntaxKind.F32LiteralToken, "0.2f32"),
        new(SyntaxKind.F32LiteralToken, "4.2f32"),
        new(SyntaxKind.F32LiteralToken, ".2e2f32"),
        new(SyntaxKind.F32LiteralToken, "0.2e2f32"),
        new(SyntaxKind.F32LiteralToken, "4.2e2f32"),
        new(SyntaxKind.F32LiteralToken, "42f32"),
        new(SyntaxKind.F64LiteralToken, ".2"),
        new(SyntaxKind.F64LiteralToken, ".2f64"),
        new(SyntaxKind.F64LiteralToken, "0.2"),
        new(SyntaxKind.F64LiteralToken, "0.2f64"),
        new(SyntaxKind.F64LiteralToken, "4.2"),
        new(SyntaxKind.F64LiteralToken, "4.2f64"),
        new(SyntaxKind.F64LiteralToken, ".2e2"),
        new(SyntaxKind.F64LiteralToken, ".2e2f64"),
        new(SyntaxKind.F64LiteralToken, "0.2e2"),
        new(SyntaxKind.F64LiteralToken, "0.2e2f64"),
        new(SyntaxKind.F64LiteralToken, "4.2e2"),
        new(SyntaxKind.F64LiteralToken, "4.2e2f64"),
        new(SyntaxKind.F64LiteralToken, "42f64"),

        new(SyntaxKind.StrLiteralToken, "\"test\""),
        new(SyntaxKind.StrLiteralToken, "\"te\\\"st\""),
    ];

    private static readonly TokenData[] s_all_tokens = Enum.GetValues<SyntaxKind>()
        .Except([SyntaxKind.BracketOpenBracketCloseToken, SyntaxKind.ParenthesisOpenParenthesisCloseToken])
        .Select(sk => new TokenData(sk, SyntaxFacts.GetText(sk)!))
        .Where(td => td.Text is not null).Concat(s_literal_tokens)
        .ToArray();

    public static TheoryData<TokenData> GetAllTokenInfo() => new(s_all_tokens);

    [Theory]
    [MemberData(nameof(GetAllTokenInfo))]
    public void Scan_scans_all_tokens(TokenData data)
    {
        var token = SyntaxTree.Scan(new SourceText(data.Text))[0];
        Assert.Equal(data.SyntaxKind, token.SyntaxKind);
    }
}
