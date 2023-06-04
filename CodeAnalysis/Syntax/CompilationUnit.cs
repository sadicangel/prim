using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class CompilationUnit(Expression Expression, Token Eof) : INode
{
    TextSpan INode.Span { get => Expression.Span; }

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true) => Expression.WriteTo(writer, indent, isLast);
    IEnumerable<INode> INode.GetChildren() => Expression.GetChildren();
}