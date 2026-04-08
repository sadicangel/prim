namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_IfExpression()
    {
        var tree = new SyntaxTree(new SourceText("if (2 > 0) true"), new ParseOptions { IsScript = true });
        var node = Assert.Single(tree.CompilationUnit.Declarations);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.IfElseExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_IfElseExpression()
    {
        var tree = new SyntaxTree(new SourceText("if (2 > 0) true else false"), new ParseOptions { IsScript = true });
        var node = Assert.Single(tree.CompilationUnit.Declarations);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.IfElseExpression, node.SyntaxKind);
    }
}
