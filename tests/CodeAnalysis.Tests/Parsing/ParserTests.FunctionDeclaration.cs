namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Fact]
    public void Parse_FunctionDeclaration_with_0_arguments()
    {
        var tree = SyntaxTree.Parse(new SourceText("f: () -> unit = {}"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.FunctionDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_FunctionDeclaration_with_1_arguments()
    {
        var tree = SyntaxTree.Parse(new SourceText("f: (x: i32) -> i32 = x;"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.FunctionDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_FunctionDeclaration_with_2_arguments()
    {
        var tree = SyntaxTree.Parse(new SourceText("f: (x: i32, y: i32) -> i32 = x + y;"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.FunctionDeclaration, node.SyntaxKind);
    }
}
