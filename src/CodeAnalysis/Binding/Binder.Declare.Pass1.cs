using System.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

partial class Binder
{
    public static void DeclarePass1(DeclarationSyntax syntax, BindingContext context)
    {
        switch (syntax.SyntaxKind)
        {
            case SyntaxKind.ModuleDeclaration:
                DeclarePass1Module((ModuleDeclarationSyntax)syntax, context);
                break;
            case SyntaxKind.StructDeclaration:
                DeclarePass1Struct((StructDeclarationSyntax)syntax, context);
                break;
            case SyntaxKind.VariableDeclaration:
                // Nothing to do here.
                break;
            default:
                throw new UnreachableException($"Unexpected {nameof(GlobalDeclarationSyntax)} '{syntax.SyntaxKind}'");
        }
    }
}
