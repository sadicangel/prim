namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Fact]
    public void Parse_EmptyExpression()
    {
        var unit = SyntaxTree.ParseScript(new SourceText(";"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.EmptyExpression, node.SyntaxKind);
    }
}
