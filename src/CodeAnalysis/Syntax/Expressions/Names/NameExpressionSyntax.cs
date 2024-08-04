namespace CodeAnalysis.Syntax.Expressions.Names;

public abstract record class NameExpressionSyntax(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
    : ExpressionSyntax(SyntaxKind, SyntaxTree);
