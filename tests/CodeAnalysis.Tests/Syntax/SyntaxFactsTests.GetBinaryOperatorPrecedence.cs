namespace CodeAnalysis.Tests.Syntax;
public sealed partial class SyntaxFactsTests
{
    private static readonly SyntaxKind[] s_binaryOperators = [
        SyntaxKind.StarStarToken,
        SyntaxKind.PercentToken,
        SyntaxKind.StarToken,
        SyntaxKind.SlashToken,
        SyntaxKind.PlusToken,
        SyntaxKind.MinusToken,
        SyntaxKind.LessThanLessThanToken,
        SyntaxKind.GreaterThanGreaterThanToken,
        SyntaxKind.EqualsEqualsToken,
        SyntaxKind.BangEqualsToken,
        SyntaxKind.LessThanToken,
        SyntaxKind.LessThanEqualsToken,
        SyntaxKind.GreaterThanToken,
        SyntaxKind.GreaterThanEqualsToken,
        SyntaxKind.AmpersandToken,
        SyntaxKind.AmpersandAmpersandToken,
        SyntaxKind.PipeToken,
        SyntaxKind.PipePipeToken,
        SyntaxKind.HatToken,
        SyntaxKind.HookHookToken,
    ];

    public static TheoryData<SyntaxKind> GetBinaryOperators() => new(s_binaryOperators);

    [Theory]
    [MemberData(nameof(GetBinaryOperators))]
    public void GetBinaryOperatorPrecedence_return_precedence_for_operator(SyntaxKind syntaxKind)
    {
        var (expression, precedence) = SyntaxFacts.GetBinaryOperatorPrecedence(syntaxKind);
        Assert.True(expression > 0);
        Assert.True(precedence > 0);
    }

    public static TheoryData<SyntaxKind> GetNonBinaryOperators() => new(Enum.GetValues<SyntaxKind>().Except(s_binaryOperators));

    [Theory]
    [MemberData(nameof(GetNonBinaryOperators))]
    public void GetBinaryOperatorPrecedence_returns_0_for_non_operator(SyntaxKind syntaxKind)
    {
        var (expression, precedence) = SyntaxFacts.GetBinaryOperatorPrecedence(syntaxKind);
        Assert.Equal((SyntaxKind)0, expression);
        Assert.Equal(0, precedence);
    }
}