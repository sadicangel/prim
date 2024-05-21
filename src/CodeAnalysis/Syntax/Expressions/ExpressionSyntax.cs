namespace CodeAnalysis.Syntax.Expressions;
public abstract record class ExpressionSyntax(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
    : SyntaxNode(SyntaxKind, SyntaxTree);
