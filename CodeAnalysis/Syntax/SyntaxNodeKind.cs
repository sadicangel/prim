namespace CodeAnalysis.Syntax;

public enum SyntaxNodeKind
{
    // Expressions
    LiteralExpression,
    UnaryExpression,
    BinaryExpression,
    GroupExpression,
    NameExpression,
    AssignmentExpression,
    IfExpression,
    CallExpression,
    ConvertExpression,

    // Statements
    GlobalStatement,
    GlobalDeclaration,
    BlockStatement,
    ExpressionStatement,
    DeclarationStatement,
    IfStatement,
    WhileStatement,
    ForStatement,
    BreakStatement,
    ContinueStatement,
    ReturnStatement,

    // Token
    Token,

    // Nodes
    Parameter,
    CompilationUnit,
}
