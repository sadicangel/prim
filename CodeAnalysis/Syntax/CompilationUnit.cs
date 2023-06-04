using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class CompilationUnit(Statement Statement, Token Eof) : INode
{
    TextSpan INode.Span { get => Statement.Span; }

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true) => Statement.WriteTo(writer, indent, isLast);
    IEnumerable<INode> INode.GetChildren() => Statement.GetChildren();
}