using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundMemberListExpression(
    SyntaxNode SyntaxNode,
    List<BoundDeclarationExpression> Members,
    PrimType Type
)
    : BoundExpression(BoundNodeKind.MemberList, SyntaxNode, Type)
{
    public override IEnumerable<BoundNode> Children() => Members;
}