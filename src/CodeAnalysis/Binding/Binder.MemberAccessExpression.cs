using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindMemberAccessExpression(MemberAccessExpressionSyntax syntax, BinderContext context)
    {
        var expression = BindExpression(syntax.Expression, context);
        if (expression.Type.IsNever)
        {
            return expression;
        }

        // TODO: Support multiple method references (overloading).
        var symbol = expression.Type
            .GetSymbols(syntax.Name.IdentifierToken.Text)
            .SingleOrDefault() ?? throw new UnreachableException($"Unexpected member '{syntax.Name.IdentifierToken.Text}'");

        return new BoundMemberReference(syntax.Name, expression, symbol, symbol.Type);
    }
}
