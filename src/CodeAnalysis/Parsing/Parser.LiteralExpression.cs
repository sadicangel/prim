﻿using System.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    public static LiteralExpressionSyntax ParseLiteralExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var literalToken = iterator.Match();

        return literalToken.SyntaxKind switch
        {
            SyntaxKind.TrueKeyword =>
                new LiteralExpressionSyntax(SyntaxKind.TrueLiteralExpression, syntaxTree, literalToken, true),
            SyntaxKind.FalseKeyword =>
                new LiteralExpressionSyntax(SyntaxKind.FalseLiteralExpression, syntaxTree, literalToken, false),
            SyntaxKind.NullKeyword =>
                new LiteralExpressionSyntax(SyntaxKind.NullLiteralExpression, syntaxTree, literalToken, Unit.Value),
            SyntaxKind.I8LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.I8LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.U8LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.U8LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.I16LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.I16LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.U16LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.U16LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.I32LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.I32LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.U32LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.U32LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.I64LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.I64LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.U64LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.U64LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.F16LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.F16LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.F32LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.F32LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.F64LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.F64LiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            SyntaxKind.StrLiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.StrLiteralExpression, syntaxTree, literalToken, literalToken.Value!),
            _ =>
                throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{literalToken.SyntaxKind}' for {nameof(LiteralExpressionSyntax)}")
        };
    }
}
