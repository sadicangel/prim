namespace CodeAnalysis.Syntax;
public static class SyntaxFactory
{
    private static readonly IReadOnlyList<SyntaxTrivia> EmptySyntaxTrivia = [];

    public static IReadOnlyList<SyntaxTrivia> GetEmptySyntaxTrivia() => EmptySyntaxTrivia;
    public static SyntaxToken GetEofToken(SyntaxTree syntaxTree) => new(SyntaxKind.EofToken, syntaxTree, new Range(syntaxTree.SourceText.Length, syntaxTree.SourceText.Length), EmptySyntaxTrivia, EmptySyntaxTrivia, "\0");
}
