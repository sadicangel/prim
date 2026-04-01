namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_QualifiedName()
    {
        var tree = new SyntaxTree(new SourceText("a::b"), new ParseOptions { IsScript = true });
        var node = Assert.Single(tree.CompilationUnit.Declarations);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.QualifiedName, node.SyntaxKind);
    }

    [Fact]
    public void Parse_QualifiedName_multiple_levels()
    {
        var tree = new SyntaxTree(new SourceText("a::b::c::d"), new ParseOptions { IsScript = true });
        var node = Assert.Single(tree.CompilationUnit.Declarations);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.QualifiedName, node.SyntaxKind);
    }
}
