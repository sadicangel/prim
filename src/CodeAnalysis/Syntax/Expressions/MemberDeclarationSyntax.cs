namespace CodeAnalysis.Syntax.Expressions;

public abstract record class MemberDeclarationSyntax(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree)
    : DeclarationSyntax(SyntaxKind, SyntaxTree);
