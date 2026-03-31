namespace CodeAnalysis.Semantic.Symbols;

internal static class SymbolExtensions
{
    extension(Symbol symbol)
    {
        // TODO: Probably need to check for multiple definitions of main.
        public VariableSymbol? FindEntryPoint()
        {
            return symbol switch
            {
                ModuleSymbol module => module.Members.Select(FindEntryPoint).FirstOrDefault(x => x is not null),
                VariableSymbol variable when IsEntryPoint(variable) => variable,
                _ => null,
            };

            static bool IsEntryPoint(VariableSymbol variable) =>
                variable is { Name: "main", Type: LambdaTypeSymbol lambdaType } && IsMainType(lambdaType);

            static bool IsMainType(LambdaTypeSymbol lambdaType) => lambdaType.ReturnType == lambdaType.ContainingModule.I32
                && lambdaType.Parameters is [ArrayTypeSymbol args] && args.ElementType == lambdaType.ContainingModule.Str;
        }
    }
}
