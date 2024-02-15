namespace CodeAnalysis.Syntax.Expressions;

public sealed record class GlobalDeclarationExpression(
    SyntaxTree SyntaxTree,
    Token Identifier,
    Token Colon,
    TypeSyntax TypeNode,
    Token Equal,
    Expression Expression
)
    : DeclarationExpression(SyntaxNodeKind.GlobalDeclarationExpression, SyntaxTree, Identifier, Colon, Mutable: null, TypeNode, Equal, Expression);