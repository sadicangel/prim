namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_StructDeclaration()
    {
        var tree = SyntaxTree.Parse(new SourceText("""
            Point2d: struct = {
                x: i32 = 0;
                y: i32 = 0;
            }
            """));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.StructDeclaration, node.SyntaxKind);
    }
}
