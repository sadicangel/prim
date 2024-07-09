using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundMethodDeclaration(
    SyntaxNode Syntax,
    FunctionSymbol FunctionSymbol,
    BoundExpression Body)
    : BoundMemberDeclaration(BoundKind.MethodDeclaration, Syntax, FunctionSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return FunctionSymbol;
        yield return Body;
    }
}
