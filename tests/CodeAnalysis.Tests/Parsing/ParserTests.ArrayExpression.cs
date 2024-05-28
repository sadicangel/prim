namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Fact]
    public void Parse_ArrayExpression()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("""
            [
                1,
                2,
                3
            ]
            """));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.ArrayExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_ArrayExpression_TrailingComma()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("""
            [
                1,
                2,
                3,
            ]
            """));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.ArrayExpression, node.SyntaxKind);
    }
}
