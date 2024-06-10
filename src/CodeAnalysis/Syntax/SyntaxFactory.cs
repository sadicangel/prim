using CodeAnalysis.Syntax.Operators;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;
public static class SyntaxFactory
{
    private static readonly SyntaxTree s_emptySyntaxTree = new(new SourceText(""), static tree => new CompilationUnitSyntax(tree, [], Token(SyntaxKind.EofToken, tree)));
    private static readonly SyntaxList<SyntaxTrivia> s_emptySyntaxTrivia = [];

    public static SyntaxList<SyntaxTrivia> EmptyTrivia() =>
        s_emptySyntaxTrivia;

    private static Range EmptyRange(SyntaxTree syntaxTree) =>
        new(syntaxTree.SourceText.Length, syntaxTree.SourceText.Length);

    public static SyntaxToken SyntheticToken(SyntaxKind syntaxKind) =>
        new SyntaxToken(syntaxKind, s_emptySyntaxTree, EmptyRange(s_emptySyntaxTree), s_emptySyntaxTrivia, s_emptySyntaxTrivia, SyntaxFacts.GetText(syntaxKind));

    public static SyntaxToken Token(SyntaxKind syntaxKind, SyntaxTree syntaxTree, string? text = null) =>
        new(syntaxKind, syntaxTree, EmptyRange(syntaxTree), s_emptySyntaxTrivia, s_emptySyntaxTrivia, text ?? SyntaxFacts.GetText(syntaxKind));

    public static OperatorSyntax Operator(SyntaxKind syntaxKind, SyntaxTree syntaxTree, int precedence = -1) =>
        new(syntaxKind, syntaxTree, Token(syntaxKind, syntaxTree), precedence);
}
