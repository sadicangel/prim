
namespace CodeAnalysis.Syntax.Expressions;

public sealed record class ArgumentSyntax(
    SyntaxTree SyntaxTree,
    ExpressionSyntax Expression)
    : SyntaxNode(SyntaxKind.Argument, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
    }
}
