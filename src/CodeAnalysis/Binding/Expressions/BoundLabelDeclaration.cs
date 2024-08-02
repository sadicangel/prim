using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLabelDeclaration(
    SyntaxNode Syntax,
    LabelSymbol LabelSymbol)
    : BoundDeclaration(BoundKind.LabelDeclaration, Syntax, LabelSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return LabelSymbol;
    }
}
