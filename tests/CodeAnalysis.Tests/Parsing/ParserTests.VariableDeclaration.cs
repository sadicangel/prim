namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Fact]
    public void Parse_VariableDeclaration_Variable()
    {
        var unit = SyntaxTree.Parse(new SourceText($"a: i32 = 2;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.VariableDeclaration, node.SyntaxKind);
    }
}
