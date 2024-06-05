namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Fact]
    public void Parse_ArrayExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("""
            [
                1,
                2,
                3
            ]
            """));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.ArrayExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_ArrayExpression_with_trailing_comma()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("""
            [
                1,
                2,
                3,
            ]
            """));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.ArrayExpression, node.SyntaxKind);
    }
}
