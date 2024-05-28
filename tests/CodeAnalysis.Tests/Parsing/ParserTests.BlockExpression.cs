using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Theory]
    [MemberData(nameof(GetBlockExpressions))]
    public void Parser_BlockExpression(int expressionCount, string source)
    {
        var unit = SyntaxTree.ParseScript(new SourceText(source));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.BlockExpression, node.SyntaxKind);
        var block = Assert.IsType<BlockExpressionSyntax>(node);
        Assert.Equal(expressionCount, block.Expressions.Count);
    }

    public static TheoryData<int, string> GetBlockExpressions()
    {
        return new TheoryData<int, string>()
        {
            {
                0, """
                {}
                """ },
            {
                1, """
                {
                    a + b;
                }
                """
            },
            {
                3, """
                {
                    a + b;
                    b + c;
                    c + d;
                }
                """
            },
        };
    }
}
