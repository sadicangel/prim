using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

partial class Binder
{
    public static void DeclarePass1Module(ModuleDeclarationSyntax syntax, BindingContext context)
    {
        var module = new ModuleSymbol(syntax, syntax.Name.FullName, context.Module);

        // Modules can be redeclared, so it's fine to return false.
        _ = context.TryDeclare(module);

        context = context with { Module = module };
        foreach (var memberSyntax in syntax.Members)
        {
            DeclarePass1Global(memberSyntax, context);
        }
    }
}
