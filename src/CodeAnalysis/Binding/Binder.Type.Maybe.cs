using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static UnionSymbol BindMaybeType(MaybeTypeSyntax syntax, BindingContext context)
    {
        var underlyingType = BindType(syntax.UnderlyingType, context);

        return new UnionSymbol(syntax, [context.Module.Unit, underlyingType], context.Module);
    }
}
