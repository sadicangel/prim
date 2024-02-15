using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal abstract record class BoundNode(
    BoundNodeKind NodeKind,
    SyntaxNode SyntaxNode
)
    : INode
{
    public abstract IEnumerable<BoundNode> Children();
    IEnumerable<INode> INode.Children() => Children();
}

internal enum BoundNodeKind
{
    DeclarationExpression,

    BlockExpression,
    EmptyExpression,

    LiteralExpression,
    GroupExpression,
    UnaryExpression,
    BinaryExpression,
    ConvertExpression,

    AssignmentExpression,
    NameExpression,

    ForExpression,
    IfElseExpression,
    WhileExpression,

    BreakExpression,
    ContinueExpression,
    ReturnExpression,

    NeverExpression,

    Symbol,
    Operator,
    ArgumentList,

    Program,
}