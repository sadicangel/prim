using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static UnionSymbol BindUnionType(UnionTypeSyntax syntax, BindingContext context)
    {
        var types = syntax.Types.Select(type => BindType(type, context)).ToImmutableArray();

        return new UnionSymbol(syntax, types, context.Module);
    }
}
