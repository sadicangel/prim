using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Names;

public abstract record class NameSyntax(SyntaxKind SyntaxKind)
    : ExpressionSyntax(SyntaxKind)
{
    public abstract string FullName { get; }
}
