﻿using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax.Expressions.Declarations;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundVariableDeclaration BindVariableDeclaration(VariableDeclarationSyntax syntax, Context context)
    {
        if (context.BoundScope.Lookup(syntax.Name.NameValue) is not VariableSymbol variableSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(VariableDeclarationSyntax)}'");

        using (context.PushBoundScope())
        {
            foreach (var symbol in variableSymbol.Type.DeclaredSymbols)
                if (!context.BoundScope.Declare(symbol))
                    throw new UnreachableException($"Failed to declare symbol '{symbol}'");

            var expression = syntax.InitValue is not null
                ? BindExpression(syntax.InitValue, context)
                : null;

            if (variableSymbol.Type.IsUnknown)
            {
                if (expression is null || expression.Type.IsUnknown)
                {
                    expression ??= new BoundNeverExpression(syntax, context.BoundScope.Never);
                    context.Diagnostics.ReportInvalidImplicitType(syntax.Location, expression.Type.Name);
                    variableSymbol = variableSymbol with { Type = context.BoundScope.Never };
                }
                else
                {
                    variableSymbol = variableSymbol with { Type = expression.Type };
                }
                if (!context.BoundScope.Replace(variableSymbol))
                {
                    throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(variableSymbol.Name));
                }
            }
            else
            {
                if (expression is null)
                {
                    if (!variableSymbol.Type.IsOption)
                    {
                        context.Diagnostics.ReportUninitializedVariable(syntax.Location, variableSymbol.Name);
                        expression = new BoundNeverExpression(syntax, context.BoundScope.Never);
                    }
                    else
                    {
                        expression = BoundLiteralExpression.Unit(context.BoundScope.Unit);
                    }
                }
                expression = Coerce(expression, variableSymbol.Type, context);
            }

            return new BoundVariableDeclaration(syntax, variableSymbol, expression);
        }
    }
}
