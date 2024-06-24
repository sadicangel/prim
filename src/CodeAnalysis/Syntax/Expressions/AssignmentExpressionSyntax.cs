using CodeAnalysis.Syntax.Operators;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class AssignmentExpressionSyntax(
    SyntaxTree SyntaxTree,
    ExpressionSyntax Left,
    OperatorSyntax Operator,
    ExpressionSyntax Right)
    : ExpressionSyntax(SyntaxKind.AssignmentExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }
}
