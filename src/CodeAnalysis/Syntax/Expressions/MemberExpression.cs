using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class MemberExpression(
    SyntaxTree SyntaxTree,
    MemberType MemberType,
    DeclarationExpression Declaration
)
    : Expression(SyntaxNodeKind.MemberExpression, SyntaxTree)
{
    public bool IsMutable { get => Declaration.IsMutable; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Declaration;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder.Node(Declaration);
    }
}

public enum MemberType
{
    Property,
    Method,
    Operator
}
