namespace CodeAnalysis.Tests.Parsing;

partial class ParserTests
{
    [Theory]
    [MemberData(nameof(UnaryExpressions))]
    public void Parse_UnaryExpression(SyntaxKind expression, string @operator)
    {
        var node = ParseExpression($"{@operator}a");
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
