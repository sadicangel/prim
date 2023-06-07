﻿using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Syntax;

public sealed record class ForStatement(Token ForToken, Token? VarToken, Token IdentifierToken, Token InToken, Expression LowerBound, Token RangeToken, Expression UpperBound, Statement Body) : Statement(NodeKind.ForStatement)
{
    [MemberNotNullWhen(true, nameof(VarToken))]
    public bool DeclaresVariable { get => VarToken is not null; }

    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Accept(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return ForToken;
        if (VarToken is not null)
            yield return VarToken;
        yield return IdentifierToken;
        yield return InToken;
        yield return LowerBound;
        yield return RangeToken;
        yield return UpperBound;
        yield return Body;
    }
}