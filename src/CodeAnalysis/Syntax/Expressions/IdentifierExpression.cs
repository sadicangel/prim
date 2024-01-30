namespace CodeAnalysis.Syntax.Expressions;

public abstract record class IdentifierExpression(
    SyntaxNodeKind SyntaxNodeKind,
    SyntaxTree SyntaxTree,
    Token Identifier,
    bool IsMutable = false
)
    : Expression(SyntaxNodeKind, SyntaxTree);
