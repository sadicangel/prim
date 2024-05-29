namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Fact]
    public void Parse_EmptyExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText(";"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.EmptyExpression, node.SyntaxKind);
    }
}
