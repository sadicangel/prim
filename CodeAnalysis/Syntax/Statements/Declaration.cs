namespace CodeAnalysis.Syntax.Statements;

public abstract record class Declaration(SyntaxTree SyntaxTree) : Statement(SyntaxNodeKind.DeclarationStatement, SyntaxTree);
