using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class ConversionDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ConversionKeyword,
    SyntaxToken ColonToken,
    FunctionTypeSyntax Type,
    SyntaxToken OperatorToken,
    ExpressionSyntax Body)
    : MemberDeclarationSyntax(SyntaxKind.ConversionDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ConversionKeyword;
        yield return ColonToken;
        yield return Type;
        yield return OperatorToken;
        yield return Body;
    }
}
