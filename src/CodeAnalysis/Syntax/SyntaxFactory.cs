namespace CodeAnalysis.Syntax;
public static class SyntaxFactory
{
    private static readonly IReadOnlyList<SyntaxTrivia> s_emptySyntaxTrivia = [];

    public static IReadOnlyList<SyntaxTrivia> EmptyTrivia() =>
        s_emptySyntaxTrivia;

    public static SyntaxToken EofToken(SyntaxTree syntaxTree) =>
        new(SyntaxKind.EofToken, syntaxTree, new Range(syntaxTree.SourceText.Length, syntaxTree.SourceText.Length), s_emptySyntaxTrivia, s_emptySyntaxTrivia, "\0");
}
