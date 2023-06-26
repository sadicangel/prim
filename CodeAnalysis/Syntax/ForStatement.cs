using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Syntax;

public sealed record class ForStatement(Token For, Token? Var, Token Identifier, Token In, Expression LowerBound, Token RangeToken, Expression UpperBound, Statement Body) : Statement(SyntaxNodeKind.ForStatement)
{
    [MemberNotNullWhen(true, nameof(Var))]
    public bool HasVariableDeclaration { get => Var is not null; }

    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return For;
        if (Var is not null)
            yield return Var;
        yield return Identifier;
        yield return In;
        yield return LowerBound;
        yield return RangeToken;
        yield return UpperBound;
        yield return Body;
    }
}
