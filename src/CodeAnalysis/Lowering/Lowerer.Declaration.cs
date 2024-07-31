using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundDeclaration LowerDeclaration(BoundDeclaration node, Context context) =>
        (BoundDeclaration)LowerExpression(node, context);
}
