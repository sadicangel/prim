using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

internal static partial class Binder
{
    public static BoundProgram Bind(IEnumerable<SyntaxTree> syntaxTrees)
    {
        var globalModule = SymbolFactory.CreateGlobalModule();

        var context = new BindingContext(globalModule, []);

        foreach (var declaration in syntaxTrees.SelectMany(tree => tree.CompilationUnit.Declarations.OfType<GlobalDeclarationSyntax>()))
        {
            DeclarePass1Global(declaration, context);
        }

        foreach (var declaration in syntaxTrees.SelectMany(tree => tree.CompilationUnit.Declarations.OfType<GlobalDeclarationSyntax>()))
        {
            DeclarePass2Global(declaration, context);
        }

        var entryPoint = globalModule.FindEntryPoint() ?? throw new NotImplementedException("entry point"); // TODO: Emit diagnostic

        return new BoundProgram(entryPoint, context.Diagnostics);
    }

    private static VariableSymbol? FindEntryPoint(this Symbol symbol)
    {
        return symbol switch
        {
            ModuleSymbol module => module.Members.Select(FindEntryPoint).FirstOrDefault(x => x is not null),
            VariableSymbol variable when IsEntryPoint(variable) => variable,
            _ => null,
        };

        static bool IsEntryPoint(VariableSymbol variable)
        {
            return variable is not null
                && variable.Name == "main"
                && variable.Type is LambdaSymbol lambda
                && lambda.ReturnType == variable.ContainingModule.I32;
        }
    }
}
