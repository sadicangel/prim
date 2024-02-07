using CodeAnalysis.Types;
using System.Text;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class DeclarationExpression(
    SyntaxTree SyntaxTree,
    Token Identifier,
    Token Colon,
    Token? Mutable,
    TypeSyntax TypeNode,
    Token Equal,
    Expression Expression
)
    : IdentifierExpression(SyntaxNodeKind.DeclarationExpression, SyntaxTree, Identifier, Mutable is not null)
{
    public DeclarationKind DeclarationKind
    {
        get => TypeNode.Type switch
        {
            FunctionType => DeclarationKind.Function,
            UserType => DeclarationKind.UserType,
            _ => DeclarationKind.Variable
        };
    }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Identifier;
        yield return Colon;
        if (Mutable is not null)
            yield return Mutable;
        yield return TypeNode;
        yield return Equal;
        yield return Expression;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .Identifier(Identifier)
            .Token(Colon)
            .Token(Mutable)
            .Node(TypeNode)
            .Token(Equal)
            .Node(Expression);
    }
}

public enum DeclarationKind
{
    Variable,
    Function,
    Operator, // TODO: Support this
    UserType,
}