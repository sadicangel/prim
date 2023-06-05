namespace CodeAnalysis.Binding;

internal enum BoundNodeKind
{
    LiteralExpression,
    UnaryExpression,
    BinaryExpression,
    VariableExpression,
    AssignmentExpression,
    IfExpression,
    BlockStatement,
    BoundDeclarationStatement,
    ExpressionStatement,
    IfStatement,
    WhileStatement,
}
