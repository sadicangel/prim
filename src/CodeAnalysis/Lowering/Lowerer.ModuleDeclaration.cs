using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundModuleDeclaration LowerModuleDeclaration(BoundModuleDeclaration node, Context context)
    {
        var declarations = LowerList(node.Declarations, context, LowerDeclaration);
        if (declarations.IsDefault)
            return node;

        return node with { Declarations = new(declarations) };
    }
}
