﻿namespace CodeAnalysis.Syntax.Expressions;

public sealed record class ConvertExpression(SyntaxTree SyntaxTree, Expression Expression, Token As, Token Type)
    : Expression(SyntaxNodeKind.ConvertExpression, SyntaxTree)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return As;
        yield return Type;
    }
    public override string ToString() => base.ToString();
}
