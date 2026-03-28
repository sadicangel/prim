using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.ControlFlow;

public sealed record class ElseClauseSyntax(
    SyntaxToken ElseKeyword,
    ExpressionSyntax Else)
    : SyntaxNode(SyntaxKind.ElseClause)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ElseKeyword;
        yield return Else;
    }
}
