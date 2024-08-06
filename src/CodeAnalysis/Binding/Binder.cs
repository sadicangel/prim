using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Binding;

internal static partial class Binder
{
    public static BoundCompilationUnit Bind(BoundTree boundTree, ScopeSymbol boundScope)
    {
        var compilationUnit = boundTree.SyntaxTree.CompilationUnit;

        if (compilationUnit.SyntaxTree.Diagnostics.HasErrorDiagnostics)
        {
            return new BoundCompilationUnit(compilationUnit, []);
        }

        var context = new Context(boundTree, boundScope);

        // TODO: Do this for all syntax trees first..
        Declare_StepOne(compilationUnit, context);
        // ..then this..
        Declare_StepTwo(compilationUnit, context);
        // ..and finally, the only thing that should be here.
        return BindCompilationUnit(compilationUnit, context);
    }
}
