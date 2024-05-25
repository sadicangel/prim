﻿using System.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static AssignmentExpressionSyntax ParseAssignmentExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        // TODO: Allow more expressions here.
        var left = ParseIdentifierNameExpression(syntaxTree, iterator);
        var operatorToken = iterator.Next();
        var syntaxKind = operatorToken.SyntaxKind switch
        {
            SyntaxKind.EqualsToken => SyntaxKind.SimpleAssignmentExpression,
            SyntaxKind.PlusEqualsToken => SyntaxKind.AddAssignmentExpression,
            SyntaxKind.MinusEqualsToken => SyntaxKind.SubtractAssignmentExpression,
            SyntaxKind.StarEqualsToken => SyntaxKind.MultiplyAssignmentExpression,
            SyntaxKind.SlashEqualsToken => SyntaxKind.DivideAssignmentExpression,
            SyntaxKind.PercentEqualsToken => SyntaxKind.ModuloAssignmentExpression,
            SyntaxKind.StarStarEqualsToken => SyntaxKind.PowerAssignmentExpression,
            SyntaxKind.LessThanLessThanEqualsToken => SyntaxKind.LeftShiftAssignmentExpression,
            SyntaxKind.GreaterThanGreaterThanEqualsToken => SyntaxKind.RightShiftAssignmentExpression,
            SyntaxKind.AmpersandEqualsToken => SyntaxKind.AndAssignmentExpression,
            SyntaxKind.PipeEqualsToken => SyntaxKind.OrAssignmentExpression,
            SyntaxKind.HatEqualsToken => SyntaxKind.ExclusiveOrAssignmentExpression,
            SyntaxKind.HookHookEqualsToken => SyntaxKind.CoalesceAssignmentExpression,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{iterator.Current.SyntaxKind}' for assignment")
        };
        var right = ParseExpression(syntaxTree, iterator, isTerminated: false);

        return new AssignmentExpressionSyntax(syntaxKind, syntaxTree, left, operatorToken, right);
    }
}
