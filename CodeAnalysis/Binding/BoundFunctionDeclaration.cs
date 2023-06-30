﻿using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundFunctionDeclaration(FunctionSymbol Function, BoundStatement Body) : BoundDeclaration(Function, BoundNodeKind.FunctionDeclaration)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Function;
        yield return Body;
    }
}