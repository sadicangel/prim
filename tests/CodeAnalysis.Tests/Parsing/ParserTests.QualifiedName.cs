namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_QualifiedName()
    {
        var node = ParseExpression("a::b");
        Assert.Equal(SyntaxKind.QualifiedName, node.SyntaxKind);
    }

    [Fact]
    public void Parse_QualifiedName_multiple_levels()
    {
        var node = ParseExpression("a::b::c::d");
        Assert.Equal(SyntaxKind.QualifiedName, node.SyntaxKind);
    }
}
