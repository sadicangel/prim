namespace CodeAnalysis.Tests.Parsing;
public partial class ParserTests
{
    [Fact]
    public void Parse_IdentifierNameExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.IdentifierNameExpression, node.SyntaxKind);
    }
}
