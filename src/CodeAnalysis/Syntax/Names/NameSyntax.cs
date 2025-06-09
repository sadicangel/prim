using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Names;

public abstract record class NameSyntax(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
    : ExpressionSyntax(SyntaxKind, SyntaxTree)
{
    public abstract string FullName { get; }
}
