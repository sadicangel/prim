using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static BoundDeclaration BindStructDeclaration(StructDeclarationSyntax syntax, BindingContext context)
    {
        var symbolName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not StructSymbol symbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(StructDeclarationSyntax)}'");

        var properties = new BoundList<BoundPropertyDeclaration>.Builder(symbol.Type.Properties.Count);
        using (context.PushScope())
        {
            // FIXME: Ensure properties are guaranteed to be declared in the same order.
            for (var i = 0; i < properties.Count; i++)
            {
                var syntaxProperty = syntax.Properties[i];
                var structProperty = symbol.Type.Properties[i];

                var propertySymbol = new PropertySymbol(syntaxProperty, structProperty.Name, structProperty.Type, structProperty.IsReadOnly);
                if (!context.BoundScope.Declare(propertySymbol))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, propertySymbol.Name);

                // TODO: Allow init expression to be optional.
                var init = BindExpression(syntaxProperty.Init, context);

                var property = new BoundPropertyDeclaration(syntaxProperty, propertySymbol, init);
                properties.Add(property);
            }
        }
        return new BoundStructDeclaration(syntax, symbol, properties.ToBoundList());
    }
}
