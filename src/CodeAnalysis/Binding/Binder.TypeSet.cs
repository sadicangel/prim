using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static TypeSymbol BindTypeSet(HashSet<TypeSymbol> types, Context context, SyntaxNode? syntax = null) => types switch
    {
        { Count: 0 } => context.BoundScope.Unknown,
        { Count: 1 } => types.Single(),
        _ when types.Contains(context.BoundScope.Never) => context.BoundScope.Never,
        _ => context.BoundScope.CreateUnionType(types, syntax),
    };
}
