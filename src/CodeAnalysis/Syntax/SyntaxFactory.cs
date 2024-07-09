namespace CodeAnalysis.Syntax;
public static class SyntaxFactory
{
    private static readonly SyntaxList<SyntaxTrivia> s_emptySyntaxTrivia = [];

    public static SyntaxList<SyntaxTrivia> EmptyTrivia() =>
        s_emptySyntaxTrivia;

    private static Range EmptyRange(SyntaxTree syntaxTree) =>
        new(syntaxTree.SourceText.Length, syntaxTree.SourceText.Length);

    public static SyntaxToken SyntheticToken(SyntaxKind syntaxKind) =>
        new(syntaxKind, SyntaxTree.Empty, EmptyRange(SyntaxTree.Empty), s_emptySyntaxTrivia, s_emptySyntaxTrivia, SyntaxFacts.GetText(syntaxKind));

    public static SyntaxToken Token(SyntaxKind syntaxKind, SyntaxTree syntaxTree, string? text = null) =>
        new(syntaxKind, syntaxTree, EmptyRange(syntaxTree), s_emptySyntaxTrivia, s_emptySyntaxTrivia, text ?? SyntaxFacts.GetText(syntaxKind));
}
