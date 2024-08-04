using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions.Declarations;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindLocalDeclaration(LocalDeclarationSyntax syntax, Context context)
    {
        Declare_StepOne(syntax.Declaration, context);
        Declare_StepTwo(syntax.Declaration, context);
        var declaration = BindDeclaration(syntax.Declaration, context);
        return declaration;
    }
}
