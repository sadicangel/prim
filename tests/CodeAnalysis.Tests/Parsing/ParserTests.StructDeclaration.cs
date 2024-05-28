namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_StructDeclaration()
    {
        var unit = SyntaxTree.Parse(new SourceText("""
            Point2d: struct : {
                x: i32 = 0;
                y: i32 = 0;
            }
            """));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.StructDeclaration, node.SyntaxKind);
    }
}
