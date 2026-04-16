namespace CodeAnalysis.Syntax.Statements;

public abstract record class StatementSyntax(SyntaxKind SyntaxKind) : SyntaxNode(SyntaxKind);
