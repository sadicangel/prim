using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax, BinderContext context)
    {
        var left = BindExpression(syntax.Left, context);

        if (left is not BoundReference @ref)
        {
            // TODO: Is this a valid error?
            throw new NotSupportedException($"Unexpected left hand side of {nameof(AssignmentExpressionSyntax)} '{syntax.Left.SyntaxKind}'");
        }

        if (@ref.Type.IsNever)
        {
            return @ref;
        }

        if (@ref.Symbol.IsReadOnly)
        {
            context.Diagnostics.ReportReadOnlyAssignment(@ref.Syntax.Location, @ref.Symbol.Name);
            return new BoundNeverExpression(syntax);
        }

        var right = @ref.Symbol is FunctionSymbol func
            ? BindFunctionAssignmentExpression(syntax.Right, func, context)
            : BindExpression(syntax.Right, context);

        right = Coerce(right, @ref.Type, context);

        if (right.Type.IsNever)
        {
            return right;
        }

        return new BoundAssignmentExpression(syntax, @ref.Type, @ref, right);
    }
}
