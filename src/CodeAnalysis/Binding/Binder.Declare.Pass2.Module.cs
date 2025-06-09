using System.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

partial class Binder
{
    public static void DeclarePass2Module(ModuleDeclarationSyntax syntax, BindingContext context)
    {
        if (!context.TryLookup<ModuleSymbol>(syntax.Name.FullName, out var module))
        {
            throw new UnreachableException($"Missing {nameof(ModuleSymbol)} '{syntax.Name.FullName}'");
        }

        context = context with { Module = module };
        foreach (var memberSyntax in syntax.Members)
        {
            DeclarePass2Global(memberSyntax, context);
        }
    }
}
