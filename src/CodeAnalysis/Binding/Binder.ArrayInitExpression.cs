using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundArrayInitExpression BindArrayInitExpression(ArrayInitExpressionSyntax syntax, BinderContext context)
    {
        var types = new HashSet<TypeSymbol>();
        var elements = new BoundList<BoundExpression>.Builder(syntax.Elements.Count);
        foreach (var elementSyntax in syntax.Elements)
        {
            var element = BindExpression(elementSyntax, context);
            elements.Add(element);
            types.Add(element.Type);
        }

        var elementType = types switch
        {
            { Count: 0 } => PredefinedSymbols.Unknown,
            { Count: 1 } => types.Single(),
            _ when types.Contains(PredefinedSymbols.Never) => PredefinedSymbols.Never,
            _ => new UnionTypeSymbol(syntax, [.. types]),
        };

        var arrayType = new ArrayTypeSymbol(syntax, elementType, elements.Count);

        return new BoundArrayInitExpression(syntax, arrayType, elements.ToBoundList());
    }
}
