using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Syntax.Types;

public sealed record class ParameterSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    TypeSyntax Type)
    : SyntaxNode(SyntaxKind.Parameter, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        yield return Type;
    }
}
