namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Fact]
    public void Parse_VariableDeclaration_of_variable()
    {
        var tree = SyntaxTree.Parse(new SourceText("a: i32 = 2;"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.VariableDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_VariableDeclaration_of_constant()
    {
        var tree = SyntaxTree.Parse(new SourceText("a: i32 = 2;"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.VariableDeclaration, node.SyntaxKind);
    }
}
