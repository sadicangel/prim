using System.Collections.Immutable;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindStructInitExpression(StructInitExpressionSyntax syntax, Context context)
    {
        if (context.BoundScope.Lookup(syntax.Name.NameValue) is not TypeSymbol typeSymbol)
        {
            context.Diagnostics.ReportUndefinedType(syntax.Location, syntax.Name.NameValue);
            return new BoundNeverExpression(syntax, context.BoundScope.Never);
        }

        var builder = ImmutableArray.CreateBuilder<BoundPropertyInitExpression>(syntax.Properties.Count);
        foreach (var propertySyntax in syntax.Properties)
        {
            if (typeSymbol.GetProperty(propertySyntax.Name.NameValue) is not PropertySymbol property)
            {
                context.Diagnostics.ReportUndefinedTypeMember(propertySyntax.Location, syntax.Name.NameValue, propertySyntax.Name.NameValue);
                continue;
            }

            var expression = BindPropertyInitExpression(propertySyntax, property, context);
            builder.Add(expression);
        }
        var properties = new BoundList<BoundPropertyInitExpression>(builder.ToImmutable());
        // TODO: Report un-initialized property members.

        return new BoundStructInitExpression(syntax, typeSymbol, properties);

        static BoundPropertyInitExpression BindPropertyInitExpression(
            PropertyInitExpressionSyntax syntax,
            PropertySymbol property,
            Context context)
        {
            var init = Coerce(BindExpression(syntax.Init, context), property.Type, context);
            var expression = new BoundPropertyInitExpression(
                syntax,
                property,
                init);
            return expression;
        }
    }
}
