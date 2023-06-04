namespace CodeAnalysis.Syntax;
public sealed class EvaluatorTests
{
    [Theory]
    [InlineData("1", 1L)]
    [InlineData("+1", 1L)]
    [InlineData("-1", -1L)]
    [InlineData("10 + 19", 29L)]
    [InlineData("12 - 3", 9L)]
    [InlineData("2 * 3", 6L)]
    [InlineData("9 / 3", 3L)]
    [InlineData("(10)", 10L)]
    [InlineData("12 == 3", false)]
    [InlineData("3 == 3", true)]
    [InlineData("12 != 3", true)]
    [InlineData("3 != 3", false)]
    [InlineData("false == false", true)]
    [InlineData("true == false", false)]
    [InlineData("false != false", false)]
    [InlineData("true != false", true)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("!true", false)]
    [InlineData("!false", true)]
    [InlineData("{ var a = 10 (a = 10) * a }", 100L)]
    public void SyntaxFacts_GetText_Roundtrips(string text, object expectedValue)
    {
        var syntaxTree = SyntaxTree.Parse(text);
        var compilation = new Compilation(syntaxTree);
        var variables = new Dictionary<Variable, object>();
        var result = compilation.Evaluate(variables);

        Assert.Empty(result.Diagnostics);

        Assert.Equal(expectedValue, result.Value);
    }

    public static IEnumerable<object[]> GetTokenKindData() => Enum.GetValues<TokenKind>().Select(e => new object[] { e });
}
