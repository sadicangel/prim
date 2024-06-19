namespace CodeAnalysis.Tests.Parsing;
public partial class ParserTests
{
    [Fact]
    public void Parse_StructExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("""
            Point2d {
                .x = 2,
                .y = 4,
            }
            """));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.StructExpression, node.SyntaxKind);
    }
}
