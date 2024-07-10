using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindIfElseExpression(IfExpressionSyntax syntax, BinderContext context)
    {
        var condition = Coerce(BindExpression(syntax.Condition, context), PredefinedTypes.Bool, context);
        if (condition.Type.IsNever)
        {
            return condition;
        }

        if (condition.ConstValue is bool isTrue)
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
        PrimType type;

        if (syntax.ElseClause is not null)
        {
            @else = BindExpression(syntax.ElseClause.Else, context);
            if (@else.Type.IsNever)
            {
                return @else;
            }

            type = then.Type == @else.Type ? then.Type : new UnionType([then.Type, @else.Type]);
        }
        else
        {
            @else = BindLiteralExpression(
                new LiteralExpressionSyntax(
                    SyntaxKind.NullLiteralExpression,
                    syntax.SyntaxTree,
                    SyntaxFactory.SyntheticToken(SyntaxKind.NullKeyword),
                    Unit.Value),
                context);
            type = then.Type.IsOption ? then.Type : new OptionType(then.Type);
        }

        return new BoundIfExpression(syntax, condition, then, @else, type);
    }
}
