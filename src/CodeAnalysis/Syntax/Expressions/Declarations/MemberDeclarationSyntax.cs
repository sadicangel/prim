namespace CodeAnalysis.Syntax.Expressions.Declarations;

public abstract record class MemberDeclarationSyntax(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree)
    : DeclarationSyntax(SyntaxKind, SyntaxTree);
