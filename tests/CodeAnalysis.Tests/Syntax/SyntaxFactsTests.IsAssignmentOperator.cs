namespace CodeAnalysis.Tests.Syntax;
public sealed partial class SyntaxFactsTests
{
    private readonly static SyntaxKind[] s_assignmentOperators = [
        SyntaxKind.EqualsToken,
        SyntaxKind.PlusEqualsToken,
        SyntaxKind.MinusEqualsToken,
        SyntaxKind.StarEqualsToken,
        SyntaxKind.SlashEqualsToken,
        SyntaxKind.PercentEqualsToken,
        SyntaxKind.StarStarEqualsToken,
        SyntaxKind.LessThanLessThanEqualsToken,
        SyntaxKind.GreaterThanGreaterThanEqualsToken,
        SyntaxKind.AmpersandEqualsToken,
        SyntaxKind.PipeEqualsToken,
        SyntaxKind.HatEqualsToken,
        SyntaxKind.HookHookEqualsToken,
    ];

    public static TheoryData<SyntaxKind> GetAssignmentOperators() =>
        new(s_assignmentOperators);

    public static TheoryData<SyntaxKind> GetNonAssignmentOperators() =>
        new(Enum.GetValues<SyntaxKind>().Except(s_assignmentOperators));

    [Theory]
    [MemberData(nameof(GetAssignmentOperators))]
    public void IsAssignmentOperator_returns_true_for_assignment_operator(SyntaxKind syntaxKind) =>
        Assert.True(SyntaxFacts.IsAssignmentOperator(syntaxKind));

    [Theory]
    [MemberData(nameof(GetNonAssignmentOperators))]
    public void IsAssignmentOperator_returns_false_for_non_assignment_operator(SyntaxKind syntaxKind) =>
        Assert.False(SyntaxFacts.IsAssignmentOperator(syntaxKind));
}
