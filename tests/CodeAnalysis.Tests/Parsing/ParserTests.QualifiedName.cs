namespace CodeAnalysis.Tests.Parsing;
public partial class ParserTests
{
    [Fact]
    public void Parse_QualifiedName()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a::b"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.QualifiedName, node.SyntaxKind);
    }

    [Fact]
    public void Parse_QualifiedName_multiple_levels()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a::b::c::d"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.QualifiedName, node.SyntaxKind);
    }
}
