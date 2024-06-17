using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundMethodDeclaration(
    SyntaxNode Syntax,
    MethodSymbol MethodSymbol,
    BoundExpression Body)
    : BoundMemberDeclaration(BoundKind.MethodDeclaration, Syntax, MethodSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return MethodSymbol;
        yield return Body;
    }
}
