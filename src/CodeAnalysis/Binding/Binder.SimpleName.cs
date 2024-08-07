﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindSimpleName(SimpleNameSyntax syntax, Context context)
    {
        if (syntax.IdentifierToken.IsSynthetic)
        {
            // Diagnostic already reported since identifier was not parsed.
            return new BoundNeverExpression(syntax, context.BoundScope.Never);
        }

        if (context.BoundScope.Lookup(syntax.NameValue) is not Symbol symbol)
        {
            context.Diagnostics.ReportUndefinedSymbol(syntax.Location, syntax.NameValue);
            return new BoundNeverExpression(syntax, context.BoundScope.Never);
        }

        return new BoundLocalReference(syntax, symbol, symbol.Type);
    }
}
