namespace CodeAnalysis.Syntax.Types;

public abstract record class TypeSyntax(SyntaxKind SyntaxKind)
    : SyntaxNode(SyntaxKind);
