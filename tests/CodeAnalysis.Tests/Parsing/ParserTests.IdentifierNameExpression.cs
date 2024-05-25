namespace CodeAnalysis.Tests.Parsing;
public partial class ParserTests
{
    [Fact]
    public void Parse_IdentifierNameExpression()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.IdentifierNameExpression, node.SyntaxKind);
    }
}
