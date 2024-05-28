namespace CodeAnalysis.Tests.Parsing;

partial class ParserTests
{
    [Theory]
    [MemberData(nameof(BinaryExpressions))]
    public void Parse_BinaryExpression(SyntaxKind expression, string @operator)
    {
        var unit = SyntaxTree.ParseScript(new SourceText($"a {@operator} b"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(expression, node.SyntaxKind);
    }

    public static TheoryData<SyntaxKind, string> BinaryExpressions()
    {
        return new TheoryData<SyntaxKind, string>
        {
            { SyntaxKind.AddExpression, "+" },
            { SyntaxKind.SubtractExpression, "-" },
            { SyntaxKind.MultiplyExpression, "*" },
            { SyntaxKind.DivideExpression, "/" },
            { SyntaxKind.ModuloExpression, "%" },
            { SyntaxKind.PowerExpression, "**" },
            { SyntaxKind.LeftShiftExpression, "<<" },
            { SyntaxKind.RightShiftExpression, ">>" },
            { SyntaxKind.LogicalOrExpression, "||" },
            { SyntaxKind.LogicalAndExpression, "&&" },
            { SyntaxKind.BitwiseOrExpression, "|" },
            { SyntaxKind.BitwiseAndExpression, "&" },
            { SyntaxKind.ExclusiveOrExpression, "^" },
            { SyntaxKind.EqualsExpression, "==" },
            { SyntaxKind.NotEqualsExpression, "!=" },
            { SyntaxKind.LessThanExpression, "<" },
            { SyntaxKind.LessThanOrEqualExpression, "<=" },
            { SyntaxKind.GreaterThanExpression, ">" },
            { SyntaxKind.GreaterThanOrEqualExpression, ">=" },
            { SyntaxKind.CoalesceExpression, "??" }
        };
    }
}
