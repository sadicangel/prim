namespace CodeAnalysis.Syntax.Expressions;

public abstract record class DeclarationSyntax(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
    : ExpressionSyntax(SyntaxKind, SyntaxTree);
