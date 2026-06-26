namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_IfExpression()
    {
        var node = ParseStatement("if (2 > 0) true");
        Assert.Equal(SyntaxKind.IfElseExpression, node.Kind);
    }

    [Fact]
    public void Parse_IfElseExpression()
    {
        var node = ParseStatement("if (2 > 0) true else false");
        Assert.Equal(SyntaxKind.IfElseExpression, node.Kind);
    }
}
