﻿using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindMemberAccessExpression(MemberAccessExpressionSyntax syntax, Context context)
    {
        var expression = BindExpression(syntax.Expression, context);
        if (expression.Type.IsNever)
        {
            return expression;
        }

        var symbols = expression.Type.GetSymbols(syntax.Name.NameValue);

        if (symbols.Count == 0)
        {
            context.Diagnostics.ReportUndefinedTypeMember(syntax.Name.Location, expression.Type.Name, syntax.Name.NameValue);
            return new BoundNeverExpression(syntax, context.BoundScope.Never);
        }

        if (symbols.Count == 1)
        {
            return symbols[0] switch
            {
                PropertySymbol property => new BoundPropertyReference(syntax.Name, expression, property),
                MethodSymbol method => new BoundMethodReference(syntax.Name, expression, method),
                _ => throw new UnreachableException($"Unexpected member symbol '{symbols[0]}'")
            };
        }

        return new BoundMethodGroup(syntax.Name, expression, [.. symbols.Cast<MethodSymbol>()]);
    }
}
