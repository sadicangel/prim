using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal interface IStatementNode : INode;

internal sealed record class DeclarationNode(SyntaxNode Syntax, Symbol Symbol, IExpressionNode? Initializer) : IStatementNode
{
    public NodeKind Kind => NodeKind.Declaration;
}

internal sealed record class ExpressionStatementNode(SyntaxNode Syntax, IExpressionNode Expression) : IStatementNode
{
    public NodeKind Kind => NodeKind.ExpressionStatement;
}
