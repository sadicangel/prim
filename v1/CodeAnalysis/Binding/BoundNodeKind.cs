namespace CodeAnalysis.Binding;

internal enum BoundNodeKind
{
    LiteralExpression,
    UnaryExpression,
    BinaryExpression,
    SymbolExpression,
    AssignmentExpression,
    CompoundAssignmentExpression,
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
    GotoStatement,
    ConditionalGotoStatement,
    LabelDeclaration,
    ReturnStatement,
    NopStatement,
}
