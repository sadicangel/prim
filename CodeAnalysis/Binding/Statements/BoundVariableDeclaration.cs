using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundVariableDeclaration(SyntaxNode Syntax, VariableSymbol Variable, BoundExpression Expression)
    : BoundDeclaration(BoundNodeKind.VariableDeclaration, Syntax, Variable)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Descendants()
    {
        yield return Variable;
        yield return Expression;
    }
}