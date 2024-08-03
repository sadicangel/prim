using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundDeclaration BindDeclaration(DeclarationSyntax syntax, Context context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.ModuleDeclaration => BindModuleDeclaration((ModuleDeclarationSyntax)syntax, context),
            SyntaxKind.StructDeclaration => BindStructDeclaration((StructDeclarationSyntax)syntax, context),
            SyntaxKind.VariableDeclaration => BindVariableDeclaration((VariableDeclarationSyntax)syntax, context),
            _ => throw new UnreachableException($"Unexpected {nameof(DeclarationSyntax)} '{syntax.GetType().Name}'")
        };
    }
}
