namespace CodeAnalysis.Tests.Parsing;

public sealed class ScannerTests
{
    [Theory]
    [InlineData(".", SyntaxKind.DotToken)]
    [InlineData("..", SyntaxKind.DotDotToken)]
    public void Scan_DotTokens(string text, SyntaxKind expectedKind)
    {
        var tokens = Scan(text);

        Assert.Collection(
            tokens,
            token => Assert.Equal(expectedKind, token.Kind),
            token => Assert.Equal(SyntaxKind.EofToken, token.Kind));
    }

    [Fact]
    public void Scan_F32LiteralToken_with_decimal_point()
    {
        var tokens = Scan("4.2f32");

        Assert.Collection(
            tokens,
            token =>
            {
                Assert.Equal(SyntaxKind.F32LiteralToken, token.Kind);
                Assert.Equal("4.2f32", token.ValueText.ToString());
            },
            token => Assert.Equal(SyntaxKind.EofToken, token.Kind));
    }

    private static SyntaxToken[] Scan(string text)
    {
        var scanner = new Scanner(new SourceText(text));
        var tokens = scanner.ScanAll().ToArray();

        Assert.Empty(scanner.Diagnostics);
        return tokens;
    }
}
