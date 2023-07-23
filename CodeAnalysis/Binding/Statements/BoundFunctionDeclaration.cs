using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundFunctionDeclaration(SyntaxNode Syntax, FunctionSymbol Function, BoundStatement Body)
    : BoundDeclaration(BoundNodeKind.FunctionDeclaration, Syntax, Function)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Function;
        yield return Body;
    }
}