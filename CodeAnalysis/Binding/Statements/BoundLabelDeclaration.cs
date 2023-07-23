using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundLabelDeclaration(SyntaxNode Syntax, LabelSymbol Label)
    : BoundDeclaration(BoundNodeKind.LabelDeclaration, Syntax, Label)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Descendants()
    {
        yield return Label;
    }
}