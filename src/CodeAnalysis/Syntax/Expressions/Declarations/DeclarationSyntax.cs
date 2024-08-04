namespace CodeAnalysis.Syntax.Expressions.Declarations;

public abstract record class DeclarationSyntax(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
    : ExpressionSyntax(SyntaxKind, SyntaxTree);
