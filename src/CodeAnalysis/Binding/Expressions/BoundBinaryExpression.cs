﻿using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundBinaryExpression(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode,
    BoundExpression Left,
    OperatorSymbol OperatorSymbol,
    BoundExpression Right)
    : BoundExpression(BoundKind, SyntaxNode, OperatorSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return OperatorSymbol;
        yield return Right;
    }
}
