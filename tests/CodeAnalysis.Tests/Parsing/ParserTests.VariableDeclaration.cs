namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Fact]
    public void Parse_VariableDeclaration_Variable()
    {
        var tree = SyntaxTree.Parse(new SourceText($"a: i32 = 2;"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.VariableDeclaration, node.SyntaxKind);
    }
}
