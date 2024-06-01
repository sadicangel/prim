using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundAssignmentExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax, BindingContext context)
    {
        var left = BindExpression(syntax.Left, context);
        var right = BindExpression(syntax.Right, context);
        var type = PredefinedTypes.Never;
        var boundKind = BoundKind.SimpleAssignmentExpression;

        if (syntax.SyntaxKind is not SyntaxKind.SimpleAssignmentExpression)
        {
            // FIXME: Implement operators
            (boundKind, right) = syntax.SyntaxKind switch
            {
                SyntaxKind.AddAssignmentExpression => (
                        BoundKind.AddAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.AddExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.SubtractAssignmentExpression => (
                        BoundKind.SubtractAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.SubtractExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.MultiplyAssignmentExpression => (
                        BoundKind.MultiplyAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.MultiplyExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.DivideAssignmentExpression => (
                        BoundKind.DivideAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.DivideExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.ModuloAssignmentExpression => (
                        BoundKind.ModuloAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.ModuloExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.PowerAssignmentExpression => (
                        BoundKind.PowerAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.PowerExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.AndAssignmentExpression => (
                        BoundKind.AndAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.LogicalAndExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.ExclusiveOrAssignmentExpression => (
                        BoundKind.ExclusiveOrAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.ExclusiveOrExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.OrAssignmentExpression => (
                        BoundKind.OrAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.LogicalAndExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.LeftShiftAssignmentExpression => (
                        BoundKind.LeftShiftAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.LeftShiftExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.RightShiftAssignmentExpression => (
                        BoundKind.RightShiftAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.RightShiftExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                SyntaxKind.CoalesceAssignmentExpression => (
                        BoundKind.CoalesceAssignmentExpression,
                        new BoundBinaryExpression(BoundKind.CoalesceExpression, syntax, PredefinedTypes.Never, left, default!, right)),
                _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
            };
        }

        return new BoundAssignmentExpression(boundKind, syntax, type, left, right);
    }
}
