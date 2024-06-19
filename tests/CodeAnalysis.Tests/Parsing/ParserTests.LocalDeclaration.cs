namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_LocalDeclaration_of_constant()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a : i32 : 10;"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.LocalDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_LocalDeclaration_of_variable()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a : i32 = 10;"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.LocalDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_LocalDeclaration_of_function()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a : () -> unit : {}"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.LocalDeclaration, node.SyntaxKind);
    }
}
