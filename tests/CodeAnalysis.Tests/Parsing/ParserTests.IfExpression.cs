namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_GroupExpression()
    {
        var node = ParseExpression("(true)");
        Assert.Equal(SyntaxKind.GroupExpression, node.SyntaxKind);
    }
}
