
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class MemberDeclarationSyntax(SyntaxTree SyntaxTree, DeclarationSyntax Declaration)
    : ExpressionSyntax(SyntaxKind.MemberDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Declaration;
    }
}
