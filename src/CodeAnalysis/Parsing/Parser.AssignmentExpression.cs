using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using System.Diagnostics;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static AssignmentExpressionSyntax ParseAssignmentExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var left = ParseExpression(syntaxTree, iterator, isTerminated: false);
        var operatorToken = iterator.Next();
        var syntaxKind = iterator.Current.SyntaxKind switch
        {
            SyntaxKind.EqualsToken => SyntaxKind.SimpleAssignmentExpression,
            SyntaxKind.PlusEqualsToken => SyntaxKind.AddAssignmentExpression,
            SyntaxKind.MinusEqualsToken => SyntaxKind.SubtractAssignmentExpression,
            SyntaxKind.StarEqualsToken => SyntaxKind.MultiplyAssignmentExpression,
            SyntaxKind.SlashEqualsToken => SyntaxKind.DivideAssignmentExpression,
            SyntaxKind.PercentEqualsToken => SyntaxKind.ModuloAssignmentExpression,
            SyntaxKind.StarStarEqualsToken => SyntaxKind.PowerAssignmentExpression,
            SyntaxKind.LessLessEqualsToken => SyntaxKind.LeftShiftAssignmentExpression,
            SyntaxKind.GreaterGreaterEqualsToken => SyntaxKind.RightShiftAssignmentExpression,
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
