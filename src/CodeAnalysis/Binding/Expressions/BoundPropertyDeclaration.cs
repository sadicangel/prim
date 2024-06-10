﻿using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundPropertyDeclaration(
    SyntaxNode SyntaxNode,
    PropertySymbol PropertySymbol,
    BoundExpression Expression)
    : BoundMemberDeclaration(BoundKind.PropertyDeclaration, SyntaxNode, PropertySymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return PropertySymbol;
        yield return Expression;
    }
}