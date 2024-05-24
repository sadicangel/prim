namespace CodeAnalysis.Tests.Parsing;
public partial class ParserTests
{
    [Fact]
    public void Parse_GroupExpression()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("(true)"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Equal(SyntaxKind.GroupExpression, node.SyntaxKind);
    }
}
