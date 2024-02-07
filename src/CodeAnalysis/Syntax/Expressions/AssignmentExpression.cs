
using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class AssignmentExpression(
    SyntaxTree SyntaxTree,
    Token Identifier,
    Token Equal,
    Expression Expression
)
    : IdentifierExpression(SyntaxNodeKind.AssignmentExpression, SyntaxTree, Identifier)
{
    public bool IsCompound { get => Equal.TokenKind is not TokenKind.Equal; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Identifier;
        yield return Equal;
        yield return Expression;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .Identifier(Identifier)
            .Token(Equal)
            .Node(Expression);
    }
}
