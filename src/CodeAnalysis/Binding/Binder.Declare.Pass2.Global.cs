using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

partial class Binder
{
    public static void DeclarePass2Global(GlobalDeclarationSyntax syntax, BindingContext context) =>
        DeclarePass2(syntax.Declaration, context, allowInference: false);
}
