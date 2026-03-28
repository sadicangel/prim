using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Declarations;

public abstract record class DeclarationSyntax(SyntaxKind SyntaxKind)
    : ExpressionSyntax(SyntaxKind);
