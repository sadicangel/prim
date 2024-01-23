namespace CodeAnalysis.Syntax;

public abstract record class GlobalSyntaxNode(SyntaxNodeKind NodeKind, SyntaxTree SyntaxTree) : SyntaxNode(NodeKind, SyntaxTree);
