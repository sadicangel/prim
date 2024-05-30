using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundDeclaration BindLocalDeclaration(LocalDeclarationSyntax syntax, BindingContext context)
    {
        Declare(syntax.Declaration, context);
        var declaration = BindDeclaration(syntax.Declaration, context);
        // We can just bind the actual declaration because LocalDeclarationSyntax
        // is just an DeclarationSyntax for a non global scope.
        return declaration;
    }
}
