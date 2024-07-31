using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundModuleDeclaration(
    SyntaxNode Syntax,
    ModuleSymbol ModuleSymbol,
    BoundList<BoundDeclaration> Declarations)
    : BoundDeclaration(BoundKind.ModuleDeclaration, Syntax, ModuleSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return ModuleSymbol;
        foreach (var declaration in Declarations)
            yield return declaration;
    }
}
