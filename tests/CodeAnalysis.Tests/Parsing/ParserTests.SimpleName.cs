namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_SimpleName()
    {
        var node = Assert.IsType<NameExpressionSyntax>(ParseExpression("a"));
        Assert.Equal(SyntaxKind.SimpleName, node.Name.Kind);
    }
}
