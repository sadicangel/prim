using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static LambdaSymbol BindLambdaType(LambdaTypeSyntax syntax, BindingContext context)
    {
        var parameters = syntax.Parameters.Select(param => BindType(param, context)).ToImmutableArray();
        var returnType = BindType(syntax.ReturnType, context);

        return new LambdaSymbol(syntax, parameters, returnType, context.Module);
    }
}
