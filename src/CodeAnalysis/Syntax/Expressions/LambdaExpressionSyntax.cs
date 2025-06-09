using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class LambdaExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken LambdaKeyword,
    SyntaxToken ParenthesisOpenToken,
    SeparatedSyntaxList<ParameterSyntax> Parameters,
    SyntaxToken ParenthesisCloseToken,
    SyntaxToken? ArrowLambdaToken,
    ExpressionSyntax Body)
    : ExpressionSyntax(SyntaxKind.LambdaExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return LambdaKeyword;
        yield return ParenthesisOpenToken;
        foreach (var parameter in Parameters.SyntaxNodes)
            yield return parameter;
        yield return ParenthesisCloseToken;
        if (ArrowLambdaToken is not null)
            yield return ArrowLambdaToken;
        yield return Body;
    }
}

public sealed record class ParameterSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameSyntax Name,
    TypeClauseSyntax? TypeClause)
    : SyntaxNode(SyntaxKind.Parameter, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        if (TypeClause is not null)
            yield return TypeClause;
    }
}
