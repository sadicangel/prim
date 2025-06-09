using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.ControlFlow;

public sealed record class ElseClauseSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ElseKeyword,
    ExpressionSyntax Else)
    : SyntaxNode(SyntaxKind.ElseClause, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ElseKeyword;
        yield return Else;
    }
}
