namespace CodeAnalysis.Tests.Parsing;
public partial class ParserTests
{
    [Fact]
    public void Parse_GroupExpression()
    {
        var tree = new SyntaxTree(new SourceText("(true)"), new ParseOptions { IsScript = true });
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.GroupExpression, node.SyntaxKind);
    }
}
