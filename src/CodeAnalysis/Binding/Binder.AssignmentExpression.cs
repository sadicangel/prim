using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax, BinderContext context)
    {
        var left = BindExpression(syntax.Left, context);

        if (left.Type.IsNever)
        {
            return left;
        }

        if (left is not BoundReference @ref)
        {
            context.Diagnostics.ReportInvalidAssignment(syntax.Location);
            return new BoundNeverExpression(syntax);
        }

        if (@ref.Symbol.IsReadOnly)
        {
            context.Diagnostics.ReportReadOnlyAssignment(@ref.Syntax.Location, @ref.Symbol.Name);
            return new BoundNeverExpression(syntax);
        }

        // TODO: Make this better. Maybe use ContainedSymbols instead?
        BoundExpression right;
        if (@ref.Symbol.Type is LambdaTypeSymbol lambdaType)
        {
            using (context.PushScope())
            {
                foreach (var parameter in lambdaType.Parameters)
                    if (!context.BoundScope.Declare(parameter))
                        throw new UnreachableException($"Failed to declare parameter '{parameter}'");
                right = Coerce(BindExpression(syntax.Right, context), lambdaType.ReturnType, context);
            }
        }
        else
        {
            right = Coerce(BindExpression(syntax.Right, context), @ref.Type, context);
        }

        if (right.Type.IsNever)
        {
            return right;
        }

        return new BoundAssignmentExpression(syntax, @ref.Type, @ref, right);
    }
}
