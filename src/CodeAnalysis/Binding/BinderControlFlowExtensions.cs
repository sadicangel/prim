using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.ControlFlow;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
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

            var body = new LoopBinder(binder).BindExpression(syntax.Body);
            return new BoundWhileExpression(syntax, condition, body);
        }

        public BoundExpression BindBreakExpression(BreakExpressionSyntax syntax)
        {
            if (!binder.IsInsideLoop())
            {
                binder.ReportInvalidBreakOrContinue(syntax.SourceSpan);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            var expression = syntax.Expression is not null
                ? binder.BindExpression(syntax.Expression)
                : binder.CreateUnitExpression(syntax.SemicolonToken ?? syntax.BreakKeyword);

            return new BoundBreakExpression(syntax, expression);
        }

        public BoundExpression BindContinueExpression(ContinueExpressionSyntax syntax)
        {
            if (!binder.IsInsideLoop())
            {
                binder.ReportInvalidBreakOrContinue(syntax.SourceSpan);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            return new BoundContinueExpression(syntax, binder.Module.Unit);
        }

        public BoundExpression BindReturnExpression(ReturnExpressionSyntax syntax)
        {
            var lambdaType = binder.GetEnclosingLambdaType();
            if (lambdaType is null)
            {
                binder.ReportInvalidReturn(syntax.SourceSpan);
                return new BoundNeverExpression(syntax, binder.Module.Never);
            }

            var expression = syntax.Expression is not null
                ? binder.BindExpression(syntax.Expression)
                : binder.CreateUnitExpression(syntax.SemicolonToken ?? syntax.ReturnKeyword);
            expression = binder.Convert(expression, lambdaType.ReturnType);

            return new BoundReturnExpression(syntax, expression);
        }

        private bool IsInsideLoop()
        {
            for (var current = binder; current is not null; current = current.Parent)
            {
                if (current is LoopBinder)
                {
                    return true;
                }
            }

            return false;
        }

        private LambdaTypeSymbol? GetEnclosingLambdaType()
        {
            for (var current = binder; current is not null; current = current.Parent)
            {
                if (current is LambdaBinder lambdaBinder)
                {
                    return lambdaBinder.LambdaType;
                }
            }

            return null;
        }

        private BoundLiteralExpression CreateUnitExpression(SyntaxNode syntax) =>
            new(syntax, binder.Module.Unit, Unit.Value);
    }
}
