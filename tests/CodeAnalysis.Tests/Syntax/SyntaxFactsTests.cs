using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using Xunit;

namespace CodeAnalysis.Tests.Syntax;
public sealed class SyntaxFactsTests
{
    [Theory]
    [MemberData(nameof(GetSyntaxKindData))]
    public void GetText_result_can_be_scanned_into_a_token(SyntaxKind syntaxKind)
    {
        var text = SyntaxFacts.GetText(syntaxKind);
        if (text is null)
            return;

        var token = Assert.Single(SyntaxTree.Scan(new SourceText(text)).SkipLast(1));
        Assert.Equal(syntaxKind, token.SyntaxKind);
        Assert.Equal(text, token.Text);
    }

    [Theory]
    [MemberData(nameof(GetAssignmentOperatorData))]
    public void IsAssignmentOperator_returns_true_for_assignment_operator(SyntaxKind syntaxKind) =>
        Assert.True(SyntaxFacts.IsAssignmentOperator(syntaxKind));

    public static TheoryData<SyntaxKind> GetSyntaxKindData() => new(Enum.GetValues<SyntaxKind>());

    public static TheoryData<SyntaxKind> GetAssignmentOperatorData() => new([
        SyntaxKind.EqualsToken,
        SyntaxKind.PlusEqualsToken,
        SyntaxKind.MinusEqualsToken,
        SyntaxKind.StarEqualsToken,
        SyntaxKind.SlashEqualsToken,
        SyntaxKind.PercentEqualsToken,
        SyntaxKind.StarStarEqualsToken,
        SyntaxKind.LessLessEqualsToken,
        SyntaxKind.GreaterGreaterEqualsToken,
        SyntaxKind.AmpersandEqualsToken,
        SyntaxKind.PipeEqualsToken,
        SyntaxKind.HatEqualsToken,
        SyntaxKind.HookHookEqualsToken,
    ]);
}
