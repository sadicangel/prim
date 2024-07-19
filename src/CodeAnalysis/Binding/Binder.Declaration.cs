using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindDeclaration(DeclarationSyntax syntax, BinderContext context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.StructDeclaration => BindStructDeclaration((StructDeclarationSyntax)syntax, context),
            SyntaxKind.FunctionDeclaration => BindFunctionDeclaration((FunctionDeclarationSyntax)syntax, context),
            SyntaxKind.VariableDeclaration => BindVariableDeclaration((VariableDeclarationSyntax)syntax, context),
            _ => throw new UnreachableException($"Unexpected {nameof(DeclarationSyntax)} '{syntax.GetType().Name}'")
        };
    }
}
