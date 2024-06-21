using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundArrayInitExpression BindArrayInitExpression(ArrayInitExpressionSyntax syntax, BinderContext context)
    {
        var types = new HashSet<PrimType>();
        var elements = new BoundList<BoundExpression>.Builder(syntax.Elements.Count);
        foreach (var elementSyntax in syntax.Elements)
        {
            var element = BindExpression(elementSyntax, context);
            elements.Add(element);
            types.Add(element.Type);
        }

        var elementType = types switch
        {
            { Count: 0 } => PredefinedTypes.Unknown,
            { Count: 1 } => types.Single(),
            _ when types.Contains(PredefinedTypes.Never) => PredefinedTypes.Never,
            _ => new UnionType([.. types]),
        };

        var arrayType = new ArrayType(elementType, elements.Count);

        return new BoundArrayInitExpression(syntax, arrayType, elements.ToBoundList());
    }
}
