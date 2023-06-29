namespace CodeAnalysis.Binding;

internal enum BoundNodeKind
{
    LiteralExpression,
    UnaryExpression,
    BinaryExpression,
    SymbolExpression,
    AssignmentExpression,
    CallExpression,
    NeverExpression,
    IfExpression,
    BlockStatement,
    VariableDeclaration,
    ExpressionStatement,
    IfStatement,
    WhileStatement,
    ForStatement,
    ConvertExpression,
    FunctionDeclaration,
}
