namespace CodeAnalysis.Text;
public sealed class SourceTextTests
{
    [Theory]
    [InlineData(".", 1)]
    [InlineData(".\r\n", 2)]
    [InlineData(".\r\n\r\n", 3)]
    public void SourceText_IncludesLastLine(string text, int expectedLineCount)
    {
        var sourceText = new SourceText(text.AsMemory());

        Assert.Equal(expectedLineCount, sourceText.Lines.Count);
    }
}
