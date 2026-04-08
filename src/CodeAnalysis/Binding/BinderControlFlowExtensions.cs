using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.ControlFlow;
using CodeAnalysis.Syntax.ControlFlow;

namespace CodeAnalysis.Binding;

internal static class BinderControlFlowExtensions
{
    extension(Binder binder)
    {
        public BoundIfElseExpression BindIfElseExpression(IfElseExpressionSyntax syntax)
        {
            var condition = binder.BindExpression(syntax.Condition);
            condition = binder.Convert(condition, binder.Module.Bool);

            var then = binder.BindExpression(syntax.Then);
            if (syntax.ElseClause is null)
            {
                return new BoundIfElseExpression(syntax, condition, then);
            }

            var @else = binder.BindExpression(syntax.ElseClause.Else);

            // TODO: We should support implicit conversions here and provide a better error message when the types are incompatible.
            var type = then.Type == @else.Type ? then.Type : binder.Module.Never;
            if (type.IsNever)
            {
                binder.ReportInvalidConversion(syntax.SourceSpan, then.Type.Name, @else.Type.Name);
            }

            return new BoundIfElseExpression(syntax, condition, then, @else, type);
        }


        public BoundWhileExpression BindWhileExpression(WhileExpressionSyntax syntax)
        {
            var condition = binder.BindExpression(syntax.Condition);
            condition = binder.Convert(condition, binder.Module.Bool);

            var body = binder.BindExpression(syntax.Body);
            return new BoundWhileExpression(syntax, condition, body);
        }
    }
}
