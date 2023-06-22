namespace CodeAnalysis.Binding;

internal enum BoundNodeKind
{
    LiteralExpression,
    UnaryExpression,
    BinaryExpression,
    VariableExpression,
    AssignmentExpression,
    CallExpression,
    NeverExpression,
    IfExpression,
    BlockStatement,
    DeclarationStatement,
    ExpressionStatement,
    IfStatement,
    WhileStatement,
    ForStatement,
    ConvertExpression,
}
