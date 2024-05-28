namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    [Theory]
    [MemberData(nameof(UnaryExpressions))]
    public void Parse_UnaryExpression(SyntaxKind expression, string @operator)
    {
        var unit = SyntaxTree.ParseScript(new SourceText($"{@operator}a"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(expression, node.SyntaxKind);
    }

    public static TheoryData<SyntaxKind, string> UnaryExpressions()
    {
        return new TheoryData<SyntaxKind, string>
        {
            { SyntaxKind.UnaryPlusExpression, "+" },
            { SyntaxKind.UnaryMinusExpression, "-" },
            { SyntaxKind.PrefixIncrementExpression, "++" },
            { SyntaxKind.PrefixDecrementExpression, "--" },
            { SyntaxKind.OnesComplementExpression, "~" },
            { SyntaxKind.NotExpression, "!" },
        };
    }
}
