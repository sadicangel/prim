using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static PointerSymbol BindPointerType(PointerTypeSyntax syntax, BindingContext context)
    {
        var elementType = BindType(syntax.ElementType, context);
        return new PointerSymbol(syntax, elementType, context.Module);
    }
}
