using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindIfElseExpression(IfExpressionSyntax syntax, Context context)
    {
        var condition = Coerce(BindExpression(syntax.Condition, context), context.BoundScope.Bool, context);
        if (condition.Type.IsNever)
        {
            return condition;
        }

        if (condition.ConstantValue is bool isTrue)
        {
            if (!isTrue)
            {
                context.Diagnostics.ReportUnreachableCode(syntax.Then.Location);
            }
            else if (syntax.ElseClause is not null)
            {
                context.Diagnostics.ReportUnreachableCode(syntax.ElseClause.Else.Location);
            }
        }

        var then = BindExpression(syntax.Then, context);
        if (then.Type.IsNever)
        {
            return then;
        }

        BoundExpression @else;
        TypeSymbol type;

        if (syntax.ElseClause is not null)
        {
            @else = BindExpression(syntax.ElseClause.Else, context);
            if (@else.Type.IsNever)
            {
                return @else;
            }

            type = then.Type == @else.Type ? then.Type : context.BoundScope.CreateUnionType([then.Type, @else.Type], syntax);
        }
        else
        {
            @else = BoundLiteralExpression.Unit(context.BoundScope.Unit);
            type = then.Type.IsOption ? then.Type : context.BoundScope.CreateOptionType(then.Type, syntax);
        }

        return new BoundIfExpression(syntax, condition, then, @else, type);
    }
}
