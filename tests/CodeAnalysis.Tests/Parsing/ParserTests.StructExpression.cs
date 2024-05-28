namespace CodeAnalysis.Tests.Parsing;
public partial class ParserTests
{
    [Fact]
    public void Parse_StructExpression()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("""
            {
                .x = 2,
                .y = 4,
            }
            """));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.StructExpression, node.SyntaxKind);
    }
}
