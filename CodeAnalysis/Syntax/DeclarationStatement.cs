using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Syntax;

public sealed record class DeclarationStatement(Token StorageToken, Token IdentifierToken, Token? ColonToken, Token? TypeToken, Token EqualsToken, Expression Expression, Token SemicolonToken) : Statement(NodeKind.DeclarationStatement)
{
    public DeclarationStatement(Token storageToken, Token identifierToken, Token equalsToken, Expression expression, Token semicolonToken)
        : this(storageToken, identifierToken, ColonToken: null, TypeToken: null, equalsToken, expression, semicolonToken) { }

    [MemberNotNullWhen(true, nameof(ColonToken), nameof(TypeToken))]
    public bool HasTypeDeclaration { get => ColonToken is not null && TypeToken is not null; }

    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Accept(this);

    public override IEnumerable<INode> GetChildren()
    {
        yield return StorageToken;
        yield return IdentifierToken;
        if (HasTypeDeclaration)
        {
            yield return ColonToken;
            yield return TypeToken;
        }
        yield return EqualsToken;
        yield return Expression;
        yield return SemicolonToken;
    }
}
