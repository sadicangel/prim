using System.Collections.Immutable;
using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions.Declarations;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundModuleDeclaration BindModuleDeclaration(ModuleDeclarationSyntax syntax, Context context)
    {
        if (context.BoundScope.Lookup(syntax.Name.NameValue) is not ModuleSymbol moduleSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(ModuleDeclarationSyntax)}'");

        var builder = ImmutableArray.CreateBuilder<BoundDeclaration>(syntax.Declarations.Count);
        using (context.PushBoundScope(moduleSymbol))
        {
            foreach (var declarationSyntax in syntax.Declarations)
            {
                var declaration = BindDeclaration(declarationSyntax, context);
                builder.Add(declaration);
            }
        }
        var declarations = new BoundList<BoundDeclaration>(builder.ToImmutable());
        return new BoundModuleDeclaration(syntax, moduleSymbol, declarations);
    }
}
