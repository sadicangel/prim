
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class PropertyDeclarationSyntax(SyntaxTree SyntaxTree, DeclarationSyntax Declaration)
    : ExpressionSyntax(SyntaxKind.PropertyDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Declaration;
    }
}
