using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateModuleDeclaration(BoundModuleDeclaration node, Context context)
    {
        var module = new ModuleValue(node.ModuleSymbol, context.Module);
        context.Module.Declare(node.ModuleSymbol, module);
        using (context.PushScope(module))
        {
            foreach (var member in node.Declarations)
            {
                _ = EvaluateExpression(member, context);
            }
        }
        return module;
    }
}
