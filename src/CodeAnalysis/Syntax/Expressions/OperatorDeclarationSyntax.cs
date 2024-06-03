using CodeAnalysis.Syntax.Operators;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class OperatorDeclarationSyntax(
    SyntaxTree SyntaxTree,
    OperatorSyntax Operator,
    SyntaxToken ColonToken,
    FunctionTypeSyntax Type,
    SyntaxToken OperatorToken,
    ExpressionSyntax Body)
    : MemberDeclarationSyntax(SyntaxKind.OperatorDeclaration, SyntaxTree)
{
    public bool IsMutable { get => OperatorToken.SyntaxKind is SyntaxKind.EqualsToken; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Operator;
        yield return ColonToken;
        yield return Type;
        yield return OperatorToken;
        yield return Body;
    }
}
