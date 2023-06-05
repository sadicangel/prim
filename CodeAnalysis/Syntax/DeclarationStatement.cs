namespace CodeAnalysis.Syntax;

public sealed record class DeclarationStatement(Token KeywordToken, Token IdentifierToken, Token EqualsToken, Expression Expression, Token SemicolonToken) : Statement(NodeKind.DeclarationStatement)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Accept(this);

    public override IEnumerable<INode> GetChildren()
    {
        yield return KeywordToken;
        yield return IdentifierToken;
        yield return EqualsToken;
        yield return Expression;
        yield return SemicolonToken;
    }
}
