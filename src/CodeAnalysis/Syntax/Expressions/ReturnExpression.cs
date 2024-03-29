﻿using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class ReturnExpression(
    SyntaxTree SyntaxTree,
    Token Return,
    Expression Result
)
    : Expression(SyntaxNodeKind.ReturnExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Return;
        yield return Result;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .ControlFlow(Return)
            .Node(Result);
    }
}
