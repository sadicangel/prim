﻿using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static StructDeclarationSyntax ParseStructDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var structToken = iterator.Match(SyntaxKind.StructKeyword);
        var operatorToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var members = ParseSyntaxList(
            syntaxTree,
            iterator,
            [SyntaxKind.BraceCloseToken, SyntaxKind.EofToken],
            ParseMemberDeclaration);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);

        return new StructDeclarationSyntax(
            syntaxTree,
            identifierToken,
            colonToken,
            structToken,
            operatorToken,
            braceOpenToken,
            members,
            braceCloseToken);
    }
}
