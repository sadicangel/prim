using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types.Metadata;

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

        var containingSymbol = new TypeSymbol(
            syntax,
            expression.Type);

        // TODO: Support multiple member references (overloading).
        var symbol = expression.Type
            .GetMembers(syntax.Name.IdentifierToken.Text)
            .Select<Member, Symbol>(m => m switch
            {
                Property property => PropertySymbol.FromProperty(property, syntax.Name),
                Method method => MethodSymbol.FromMethod(method, expression.Type),
                Operator @operator => MethodSymbol.FromOperator(@operator),
                Conversion conversion => MethodSymbol.FromConversion(conversion),
                _ => throw new UnreachableException($"Unexpected member '{syntax.Name.IdentifierToken.Text}'"),
            })
            .SingleOrDefault() ?? throw new UnreachableException($"Unexpected member '{syntax.Name.IdentifierToken.Text}'");

        return new BoundMemberReference(syntax.Name, expression, symbol);
    }
}
