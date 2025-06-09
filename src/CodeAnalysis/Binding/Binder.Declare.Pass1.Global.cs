using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

partial class Binder
{
    public static void DeclarePass1Global(GlobalDeclarationSyntax syntax, BindingContext context) =>
        DeclarePass1(syntax.Declaration, context);
}
