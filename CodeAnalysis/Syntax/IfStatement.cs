using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Syntax;

public sealed record class IfStatement(Token IfToken, Expression Condition, Statement Then, Token? ElseToken, Statement? Else) : Statement(NodeKind.IfStatement)
{
    public IfStatement(Token ifToken, Expression condition, Statement then) : this(ifToken, condition, then, ElseToken: null, Else: null) { }

    [MemberNotNullWhen(true, nameof(ElseToken), nameof(Else))]
    public bool HasElseClause { get => ElseToken is not null && Else is not null; }

    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Accept(this);
    public override IEnumerable<Node> GetChildren()
    {
        yield return IfToken;
        yield return Condition;
        yield return Then;
        if (HasElseClause)
        {
            yield return ElseToken;
            yield return Else;
        }
    }
}