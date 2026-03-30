using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Declarations;

internal sealed record class BoundVariableDeclaration(
    SyntaxNode Syntax,
    VariableSymbol VariableSymbol,
    BoundExpression Expression)
    : BoundDeclaration(BoundKind.VariableDeclaration, Syntax, VariableSymbol.Type)
{
    public override IEnumerable<ITreeNode> Children()
    {
        yield return VariableSymbol;
        yield return Expression;
    }
}
