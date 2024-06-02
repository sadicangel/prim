using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindStructExpression(StructExpressionSyntax syntax, BindingContext context)
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
            var propertySymbol = new PropertySymbol(propertySyntax, property);
            var init = BindExpression(propertySyntax.Init, context);
            var propExpr = new BoundPropertyExpression(propertySyntax, propertySymbol, init);
            properties.Add(propExpr);
        }
        return new BoundStructExpression(syntax, PredefinedTypes.Unknown, properties.ToBoundList());
    }
}
