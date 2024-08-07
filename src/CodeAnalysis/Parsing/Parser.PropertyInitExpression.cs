﻿using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static PropertyInitExpressionSyntax ParsePropertyInitExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var dotToken = iterator.Match(SyntaxKind.DotToken);
        var name = ParseSimpleName(syntaxTree, iterator);
        var equalsToken = iterator.Match(SyntaxKind.EqualsToken);
        var init = ParseExpression(syntaxTree, iterator);
        return new PropertyInitExpressionSyntax(syntaxTree, dotToken, name, equalsToken, init);
    }
}
