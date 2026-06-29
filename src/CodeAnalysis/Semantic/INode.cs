using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal interface INode
{
    NodeKind Kind { get; }
    SyntaxNode Syntax { get; }
}
