using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class ConversionDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ConversionKeyword,
    SyntaxToken ColonToken,
    LambdaTypeSyntax Type,
    SyntaxToken ColonOrEqualsToken,
    ExpressionSyntax Body)
    : MemberDeclarationSyntax(SyntaxKind.ConversionDeclaration, SyntaxTree)
{
    public bool IsReadOnly { get => ColonOrEqualsToken.SyntaxKind is SyntaxKind.ColonToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ConversionKeyword;
        yield return ColonToken;
        yield return Type;
        yield return ColonOrEqualsToken;
        yield return Body;
    }
}
