﻿using System.Diagnostics;
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
        var memberName = syntax.Name.IdentifierToken.Text.ToString();
        Symbol memberSymbol = expression.Type.GetMember(memberName) switch
        {
            Property property => new PropertySymbol(syntax.Name, property, property.IsReadOnly, property.IsStatic),
            Method method => FunctionSymbol.FromMethod(method, expression.Type),
            Operator @operator => new OperatorSymbol(syntax.Name, @operator),
            Conversion conversion => FunctionSymbol.FromConversion(conversion),
            _ => throw new UnreachableException($"Unexpected member '{syntax.Name.IdentifierToken.Text}'"),
        };

        return new BoundMemberReference(syntax.Name, expression, memberSymbol);
    }
}
