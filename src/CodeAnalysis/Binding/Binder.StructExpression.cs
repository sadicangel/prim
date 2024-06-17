﻿using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindStructExpression(StructExpressionSyntax syntax, BinderContext context)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(structName) is not StructSymbol structSymbol)
        {
            context.Diagnostics.ReportUndefinedType(syntax.Location, structName);
            return new BoundNeverExpression(syntax);
        }

        var properties = new BoundList<BoundPropertyExpression>.Builder(syntax.Properties.Count);
        foreach (var propertySyntax in syntax.Properties)
        {
            if (structSymbol.Type.GetProperty(propertySyntax.IdentifierToken.Text) is not Property property)
            {
                context.Diagnostics.ReportUndefinedTypeMember(propertySyntax.Location, structName, propertySyntax.IdentifierToken.Text.ToString());
                continue;
            }

            var init = Convert(BindExpression(propertySyntax.Init, context), property.Type, isExplicit: false, context);
            var expression = new BoundPropertyExpression(propertySyntax, new PropertySymbol(propertySyntax, property), init);
            properties.Add(expression);
        }
        // TODO: Report un-initialized property members.
        return new BoundStructExpression(syntax, structSymbol, properties.ToBoundList());
    }
}
