using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions.Declarations;
public sealed record class ConversionDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ConversionKeyword,
    SyntaxToken ColonToken,
    LambdaTypeSyntax Type,
    SyntaxToken EqualsToken,
    ExpressionSyntax Body)
    : MemberDeclarationSyntax(SyntaxKind.ConversionDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ConversionKeyword;
        yield return ColonToken;
        yield return Type;
        yield return EqualsToken;
        yield return Body;
    }
}
