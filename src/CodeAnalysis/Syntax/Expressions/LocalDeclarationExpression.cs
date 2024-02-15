namespace CodeAnalysis.Syntax.Expressions;

public sealed record class LocalDeclarationExpression(
    SyntaxTree SyntaxTree,
    Token Identifier,
    Token Colon,
    Token? Mutable,
    TypeSyntax TypeNode,
    Token Equal,
    Expression Expression
)
    : DeclarationExpression(SyntaxNodeKind.LocalDeclarationExpression, SyntaxTree, Identifier, Colon, Mutable, TypeNode, Equal, Expression);