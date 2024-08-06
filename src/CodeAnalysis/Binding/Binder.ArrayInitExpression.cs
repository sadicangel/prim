using System.Collections.Immutable;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundArrayInitExpression BindArrayInitExpression(ArrayInitExpressionSyntax syntax, Context context)
    {
        var types = new HashSet<TypeSymbol>();
        var builder = ImmutableArray.CreateBuilder<BoundExpression>(syntax.Elements.Count);
        foreach (var elementSyntax in syntax.Elements)
        {
            var element = BindExpression(elementSyntax, context);
            builder.Add(element);
            types.Add(element.Type);
        }

        var elements = new BoundList<BoundExpression>(builder.ToImmutable());

        var elementType = BindTypeSet(types, context, syntax);

        var arrayType = new ArrayTypeSymbol(syntax, elementType, elements.Count, context.Module);

        return new BoundArrayInitExpression(syntax, arrayType, elements);
    }
}
