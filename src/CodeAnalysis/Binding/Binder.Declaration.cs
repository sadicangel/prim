using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundDeclaration BindDeclaration(DeclarationSyntax syntax, BindingContext context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.VariableDeclaration => BindVariableDeclaration((VariableDeclarationSyntax)syntax, context),
            SyntaxKind.FunctionDeclaration => BindFunctionDeclaration((FunctionDeclarationSyntax)syntax, context),
            SyntaxKind.StructDeclaration => BindStructDeclaration((StructDeclarationSyntax)syntax, context),
            _ => throw new UnreachableException($"Unexpected {nameof(DeclarationSyntax)} '{syntax.GetType().Name}'")
        };
    }
}
