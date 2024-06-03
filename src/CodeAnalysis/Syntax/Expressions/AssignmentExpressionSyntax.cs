using CodeAnalysis.Syntax.Operators;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class AssignmentExpressionSyntax(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    ExpressionSyntax Left,
    OperatorSyntax Operator,
    ExpressionSyntax Right)
    : ExpressionSyntax(SyntaxKind, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }
}
