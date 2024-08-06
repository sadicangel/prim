namespace CodeAnalysis.Syntax.Expressions.Names;

public abstract record class NameSyntax(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
    : ExpressionSyntax(SyntaxKind, SyntaxTree)
{
    public abstract NameValue NameValue { get; }
}
