﻿using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundVariableDeclaration(
    SyntaxNode SyntaxNode,
    VariableSymbol Symbol,
    BoundExpression Expression)
    : BoundDeclaration(BoundKind.VariableDeclaration, SyntaxNode)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
    }
}