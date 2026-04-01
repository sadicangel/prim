namespace CodeAnalysis.Tests.Parsing;

partial class ParserTests
{
    [Theory]
    [MemberData(nameof(UnaryExpressions))]
    public void Parse_UnaryExpression(SyntaxKind expression, string @operator)
    {
        var tree = new SyntaxTree(new SourceText($"{@operator}a"), new ParseOptions { IsScript = true });
        var node = Assert.Single(tree.CompilationUnit.Declarations);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(expression, node.SyntaxKind);
    }

    public static TheoryData<SyntaxKind, string> UnaryExpressions()
    {
        return new TheoryData<SyntaxKind, string>
        {
            { SyntaxKind.UnaryPlusExpression, "+" },
            { SyntaxKind.UnaryMinusExpression, "-" },
            { SyntaxKind.OnesComplementExpression, "~" },
            { SyntaxKind.NotExpression, "!" },
        };
    }
}
