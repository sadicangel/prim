namespace CodeAnalysis.Syntax;
public static class SyntaxFactory
{
    private static readonly SyntaxList<SyntaxTrivia> s_emptySyntaxTrivia = [];

    public static SyntaxList<SyntaxTrivia> EmptyTrivia() =>
        s_emptySyntaxTrivia;

    private static Range EmptyRange(SyntaxTree syntaxTree) =>
        new(syntaxTree.SourceText.Length, syntaxTree.SourceText.Length);

    public static SyntaxToken SyntheticToken(SyntaxKind syntaxKind) =>
        SyntheticToken(syntaxKind, SyntaxTree.Empty);

    public static SyntaxToken SyntheticToken(SyntaxKind syntaxKind, SyntaxTree syntaxTree) =>
        SyntheticToken(syntaxKind, syntaxTree, EmptyRange(syntaxTree));

    public static SyntaxToken SyntheticToken(SyntaxKind syntaxKind, SyntaxTree syntaxTree, Range range) =>
        new(syntaxKind, syntaxTree, range, s_emptySyntaxTrivia, s_emptySyntaxTrivia, Value: null);
}
