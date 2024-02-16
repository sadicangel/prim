using System.Text;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class LocalDeclarationExpression(
    SyntaxTree SyntaxTree,
    DeclarationExpression Declaration
)
    : IdentifierExpression(SyntaxNodeKind.LocalDeclarationExpression, SyntaxTree, Declaration.Identifier, Declaration.IsMutable)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Declaration;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder.Node(Declaration);
    }
}