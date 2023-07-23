using CodeAnalysis.Syntax.Expressions;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Syntax.Statements;

public sealed record class IfStatement(SyntaxTree SyntaxTree, Token If, Token OpenParenthesis, Expression Condition, Token CloseParenthesis, Statement Then, Token? ElseToken, Statement? Else)
    : Statement(SyntaxNodeKind.IfStatement, SyntaxTree)
{
    public IfStatement(SyntaxTree syntaxTree, Token ifToken, Token openParenthesis, Expression condition, Token closeParenthesis, Statement then)
        : this(syntaxTree, ifToken, openParenthesis, condition, closeParenthesis, then, ElseToken: null, Else: null) { }

    [MemberNotNullWhen(true, nameof(ElseToken), nameof(Else))]
    public bool HasElseClause { get => ElseToken is not null && Else is not null; }

    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return If;
        yield return Condition;
        yield return Then;
        if (HasElseClause)
        {
            yield return ElseToken;
            yield return Else;
        }
    }
    public override string ToString() => base.ToString();
}