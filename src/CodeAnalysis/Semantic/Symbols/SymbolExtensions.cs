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
                variable is { Name: "main", Type: LambdaSymbol lambda } && IsMainType(lambda);

            static bool IsMainType(LambdaSymbol lambda) => lambda.ReturnType == lambda.ContainingModule.I32
                && lambda.Parameters is [ArraySymbol args] && args.ElementType == lambda.ContainingModule.Str;
        }
    }
}
