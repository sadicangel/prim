namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Theory]
    [MemberData(nameof(UnaryExpressions))]
    public void Parse_UnaryExpression(SyntaxKind expression, string @operator)
    {
        var tree = SyntaxTree.ParseScript(new SourceText($"{@operator}a"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
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
