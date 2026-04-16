using CodeAnalysis.Syntax.Statements;

namespace CodeAnalysis.Syntax.Declarations;

public abstract record class DeclarationSyntax(SyntaxKind SyntaxKind) : StatementSyntax(SyntaxKind);
