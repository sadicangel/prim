
namespace CodeAnalysis.Syntax.Operators;
public sealed record class OperatorSyntax(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    SyntaxToken OperatorToken,
    int Precedence)
    : SyntaxNode(SyntaxKind, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return OperatorToken;
    }
}
