namespace CodeAnalysis.Tests.Parsing;
public partial class ParserTests
{
    [Fact]
    public void Parse_SimpleName()
    {
        var tree = new SyntaxTree(new SourceText("a"), new ParseOptions { IsScript = true });
        var node = Assert.Single(tree.CompilationUnit.Declarations);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.SimpleName, node.SyntaxKind);
    }
}
