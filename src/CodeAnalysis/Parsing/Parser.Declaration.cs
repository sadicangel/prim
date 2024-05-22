﻿using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static DeclarationSyntax ParseDeclaration(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        return iterator.Peek(2).SyntaxKind switch
        {
            SyntaxKind.TypeKeyword => ParseStructDeclaration(syntaxTree, iterator),
            SyntaxKind.ParenthesisOpenToken => ParseFunctionDeclaration(syntaxTree, iterator),
            _ => ParseVariableDeclaration(syntaxTree, iterator),
        };
    }
}