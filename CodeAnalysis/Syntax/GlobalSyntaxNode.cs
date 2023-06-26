namespace CodeAnalysis.Syntax;

public abstract record class GlobalSyntaxNode(SyntaxNodeKind NodeKind) : SyntaxNode(NodeKind);
