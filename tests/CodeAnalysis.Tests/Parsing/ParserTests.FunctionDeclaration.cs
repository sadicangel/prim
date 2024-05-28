namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Fact]
    public void Parse_FunctionDeclaration_0_Args()
    {
        var unit = SyntaxTree.Parse(new SourceText("f: () -> unit = {}"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.FunctionDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_FunctionDeclaration_1_Args()
    {
        var unit = SyntaxTree.Parse(new SourceText("f: (x: i32) -> i32 = x;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.FunctionDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_FunctionDeclaration_2_Args()
    {
        var unit = SyntaxTree.Parse(new SourceText("f: (x: i32, y: i32) -> i32 = x + y;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.FunctionDeclaration, node.SyntaxKind);
    }
}
