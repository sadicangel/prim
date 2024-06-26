﻿using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParsePrimaryExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var current = iterator.Current;
        return current.SyntaxKind switch
        {
            >= SyntaxKind.TrueKeyword and <= SyntaxKind.NullKeyword => ParseLiteralExpression(syntaxTree, iterator),
            >= SyntaxKind.I32LiteralToken and <= SyntaxKind.StrLiteralToken => ParseLiteralExpression(syntaxTree, iterator),
            SyntaxKind.ParenthesisOpenToken => ParseGroupExpression(syntaxTree, iterator),
            _ => iterator.Peek(1).SyntaxKind switch
            {
                SyntaxKind.ColonToken => ParseLocalDeclaration(syntaxTree, iterator),
                SyntaxKind.EqualsToken => ParseAssignmentExpression(syntaxTree, iterator),
                _ => ParseIdentifierNameExpression(syntaxTree, iterator),
            }
        };
    }
}
