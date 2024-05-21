﻿using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static VariableDeclarationSyntax ParseVariableDeclaration(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var type = ParseType(syntaxTree, iterator);
        var operatorToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
        var expression = ParseExpression(syntaxTree, iterator, isTerminated: true);

        return new VariableDeclarationSyntax(
            syntaxTree,
            identifierToken,
            colonToken,
            type,
            operatorToken,
            expression);
    }
}
