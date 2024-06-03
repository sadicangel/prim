using CodeAnalysis.Syntax.Operators;

namespace CodeAnalysis.Syntax;
public static class SyntaxFactory
{
    private static readonly SyntaxList<SyntaxTrivia> s_emptySyntaxTrivia = [];

    public static SyntaxList<SyntaxTrivia> EmptyTrivia() =>
        s_emptySyntaxTrivia;

    private static Range EmptyRange(SyntaxTree syntaxTree) =>
        new(syntaxTree.SourceText.Length, syntaxTree.SourceText.Length);

    public static SyntaxToken Token(SyntaxKind syntaxKind, SyntaxTree syntaxTree, string? text = null) =>
        new(syntaxKind, syntaxTree, EmptyRange(syntaxTree), s_emptySyntaxTrivia, s_emptySyntaxTrivia, text ?? SyntaxFacts.GetText(syntaxKind));

    public static OperatorSyntax Operator(SyntaxKind syntaxKind, SyntaxTree syntaxTree, int precedence = -1) =>
        new(syntaxKind, syntaxTree, Token(syntaxKind, syntaxTree), precedence);
}
