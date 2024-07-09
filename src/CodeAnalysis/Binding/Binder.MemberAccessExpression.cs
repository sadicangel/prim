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

        // TODO: Support multiple member references (overloading).
        var symbol = expression.Type
            .GetMembers(syntax.Name.IdentifierToken.Text)
            .Select<Member, Symbol>(m => m switch
            {
                Property property => new PropertySymbol(syntax.Name, property, property.IsReadOnly, property.IsStatic),
                Method method => FunctionSymbol.FromMethod(method, expression.Type),
                Operator @operator => FunctionSymbol.FromOperator(@operator),
                Conversion conversion => FunctionSymbol.FromConversion(conversion),
                _ => throw new UnreachableException($"Unexpected member '{syntax.Name.IdentifierToken.Text}'"),
            })
            .SingleOrDefault() ?? throw new UnreachableException($"Unexpected member '{syntax.Name.IdentifierToken.Text}'");

        return new BoundMemberReference(syntax.Name, expression, symbol);
    }
}
