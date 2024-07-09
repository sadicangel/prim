namespace CodeAnalysis.Tests.Syntax;
public sealed partial class SyntaxFactsTests
{
    private static readonly SyntaxKind[] s_unaryOperators = [
        SyntaxKind.BangToken,
        SyntaxKind.MinusToken,
        SyntaxKind.PlusToken,
        SyntaxKind.TildeToken,
    ];

    public static TheoryData<SyntaxKind> GetUnaryOperators() => new(s_unaryOperators);

    [Theory]
    [MemberData(nameof(GetUnaryOperators))]
    public void GetUnaryOperatorPrecedence_return_precedence_for_operator(SyntaxKind syntaxKind)
    {
        var precedence = SyntaxFacts.GetUnaryOperatorPrecedence(syntaxKind);
        Assert.True(precedence > 0);
    }

    public static TheoryData<SyntaxKind> GetNonUnaryOperators() => new(Enum.GetValues<SyntaxKind>().Except(s_unaryOperators));

    [Theory]
    [MemberData(nameof(GetNonUnaryOperators))]
    public void GetUnaryOperatorPrecedence_returns_0_for_non_operator(SyntaxKind syntaxKind)
    {
        var precedence = SyntaxFacts.GetUnaryOperatorPrecedence(syntaxKind);
        Assert.Equal(0, precedence);
    }
}
