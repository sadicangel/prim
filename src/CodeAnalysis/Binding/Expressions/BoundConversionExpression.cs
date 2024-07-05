﻿using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundConversionExpression(
    SyntaxNode Syntax,
    FunctionSymbol ConversionSymbol,
    BoundExpression Expression)
    : BoundExpression(BoundKind.ConversionExpression, Syntax, ConversionSymbol.Type.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return ConversionSymbol;
        yield return Expression;
    }
}
