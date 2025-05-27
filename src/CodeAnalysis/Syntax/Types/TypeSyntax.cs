namespace CodeAnalysis.Syntax.Types;
public abstract record class TypeSyntax(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
    : SyntaxNode(SyntaxKind, SyntaxTree);
