namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_LocalDeclaration_Constant()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a : i32 : 10;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.LocalDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_LocalDeclaration_Variable()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a : i32 = 10;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.LocalDeclaration, node.SyntaxKind);
    }

    [Fact]
    public void Parse_LocalDeclaration_Function()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a : () -> unit : {}"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.LocalDeclaration, node.SyntaxKind);
    }
}
