using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Declarations;
public abstract record class DeclarationSyntax(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
    : ExpressionSyntax(SyntaxKind, SyntaxTree);
