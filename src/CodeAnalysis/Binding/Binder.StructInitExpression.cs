using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindStructInitExpression(StructInitExpressionSyntax syntax, Context context)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(structName) is not TypeSymbol typeSymbol)
        {
            context.Diagnostics.ReportUndefinedType(syntax.Location, structName);
            return new BoundNeverExpression(syntax);
        }

        var properties = new BoundList<BoundPropertyInitExpression>.Builder(syntax.Properties.Count);
        foreach (var propertySyntax in syntax.Properties)
        {
            if (typeSymbol.GetProperty(propertySyntax.IdentifierToken.Text) is not PropertySymbol property)
            {
                context.Diagnostics.ReportUndefinedTypeMember(propertySyntax.Location, structName, propertySyntax.IdentifierToken.Text.ToString());
                continue;
            }

            if (property.IsReadOnly)
            {
                context.Diagnostics.ReportReadOnlyAssignment(propertySyntax.Location, property.Name);
                continue;
            }

            var expression = BindPropertyInitExpression(propertySyntax, property, context);
            properties.Add(expression);
        }

        // TODO: Report un-initialized property members.
        return new BoundStructInitExpression(syntax, typeSymbol, properties.ToBoundList());

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
