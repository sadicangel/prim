﻿using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static StructDeclarationSyntax ParseStructDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var name = ParseSimpleNameExpression(syntaxTree, iterator);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var structKeyword = iterator.Match(SyntaxKind.StructKeyword);
        var colonOrEquals = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var members = ParseSyntaxList(
            syntaxTree,
            iterator,
            [SyntaxKind.BraceCloseToken, SyntaxKind.EofToken],
            ParseMemberDeclaration);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);

        return new StructDeclarationSyntax(
            syntaxTree,
            name,
            colonToken,
            structKeyword,
            colonOrEquals,
            braceOpenToken,
            members,
            braceCloseToken);
    }
}
