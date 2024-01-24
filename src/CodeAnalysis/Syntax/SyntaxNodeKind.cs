namespace CodeAnalysis.Syntax;

public enum SyntaxNodeKind
{
    // Expressions
    AssignmentExpression,
    CompoundAssignmentExpression,

    LiteralExpression,
    GroupExpression,
    UnaryExpression,
    BinaryExpression,
    ConvertExpression,
    NameExpression,
    CallExpression,

    BlockExpression,
    ForExpression,
    IfExpression,
    WhileExpression,

    ResultExpression,
    BreakExpression,
    ContinueExpression,
    ReturnExpression,

    // Token
    Token,

    // Trivia
    Trivia,

    // Nodes
    Parameter,
    CompilationUnit,
}