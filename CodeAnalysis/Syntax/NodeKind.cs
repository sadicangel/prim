namespace CodeAnalysis.Syntax;

public enum NodeKind
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
    BlockStatement,
    ExpressionStatement,
    DeclarationStatement,
    IfStatement,
    WhileStatement,
    ForStatement,

    // Token
    Token,

    // Compilation Unit
    CompilationUnit,
}
