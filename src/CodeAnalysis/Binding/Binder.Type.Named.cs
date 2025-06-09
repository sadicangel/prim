using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static StructSymbol BindNamedType(NamedTypeSyntax syntax, BindingContext context)
    {
        if (!context.Module.TryLookup<StructSymbol>(syntax.Name.FullName, out var @struct))
        {
            context.Diagnostics.ReportUndefinedType(syntax.Name.SourceSpan, syntax.Name.FullName);

            return context.Module.Never;
        }

        return @struct;
    }
}
