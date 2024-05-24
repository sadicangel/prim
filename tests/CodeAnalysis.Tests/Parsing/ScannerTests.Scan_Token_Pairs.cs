namespace CodeAnalysis.Tests.Parsing;

#if !DEBUG
public partial class ScannerTests
{
    public static TheoryData<TokenData, TokenData, TokenData> GetSeparatedTokenPairs()
    {
        var data = new TheoryData<TokenData, TokenData, TokenData>();
        foreach (var x in s_all_tokens)
            foreach (var y in s_all_tokens)
                if (RequireSeparator(x.SyntaxKind, y.SyntaxKind))
                    foreach (var t in s_trivia)
                        if (!RequireSeparator(x.SyntaxKind, t.SyntaxKind) && !RequireSeparator(t.SyntaxKind, y.SyntaxKind))
                            data.Add(x, t, y);
        return data;
    }

    [Theory]
    [MemberData(nameof(GetSeparatedTokenPairs))]
    public void Scan_scans_all_separated_pairs(TokenData x, TokenData t, TokenData y)
    {
        if (SyntaxTree.Scan(new SourceText(x.Text + t.Text + y.Text)) is [var a, var b, _])
        {
            Assert.Equal(x.SyntaxKind, a.SyntaxKind);
            Assert.Equal(x.Text, a.Text);
            Assert.Equal(y.SyntaxKind, b.SyntaxKind);
            Assert.Equal(y.Text, b.Text);
        }
        else
        {
            Assert.Fail($"{nameof(Scan_scans_all_pairs)} must scan 3 tokens");
        }
    }
}
#endif
