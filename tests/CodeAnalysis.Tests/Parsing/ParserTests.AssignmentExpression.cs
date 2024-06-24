namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_SimpleAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a = 3"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.AssignmentExpression, node.SyntaxKind);
    }
}
