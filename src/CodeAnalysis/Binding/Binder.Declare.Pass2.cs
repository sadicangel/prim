using System.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

partial class Binder
{
    public static void DeclarePass2(DeclarationSyntax syntax, BindingContext context, bool allowInference)
    {
        switch (syntax.SyntaxKind)
        {
            case SyntaxKind.ModuleDeclaration:
                DeclarePass2Module((ModuleDeclarationSyntax)syntax, context);
                break;
            case SyntaxKind.StructDeclaration:
                DeclarePass2Struct((StructDeclarationSyntax)syntax, context);
                break;
            case SyntaxKind.VariableDeclaration:
                DeclarePass2Variable((VariableDeclarationSyntax)syntax, context, allowInference);
                break;
            default:
                throw new UnreachableException($"Unexpected {nameof(GlobalDeclarationSyntax)} '{syntax.SyntaxKind}'");
        }
    }
}
