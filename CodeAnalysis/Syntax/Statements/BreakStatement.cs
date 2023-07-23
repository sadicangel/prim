﻿namespace CodeAnalysis.Syntax.Statements;

public sealed record class BreakStatement(SyntaxTree SyntaxTree, Token Break, Token Semicolon) : Statement(SyntaxNodeKind.BreakStatement, SyntaxTree)
{
    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Break;
        yield return Semicolon;
    }
    public override string ToString() => base.ToString();
}
