namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_LocalDeclaration_Constant()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a : i32 : 10;"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.LocalDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_LocalDeclaration_Variable()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a : i32 = 10;"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.LocalDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_LocalDeclaration_Function()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a : () -> unit : {}"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.LocalDeclaration, node.SyntaxKind);
    }
}
