namespace CodeAnalysis.Tests.Text;
public sealed class SourceTextTests
{
    [Theory]
    [InlineData(".", 1)]
    [InlineData(".\r\n", 2)]
    [InlineData(".\r\n\r\n", 3)]
    public void SourceText_has_correct_number_of_lines(string text, int expectedLineCount)
        => Assert.Equal(expectedLineCount, new SourceText(text).Lines.Count);
}
