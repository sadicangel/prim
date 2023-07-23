using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal abstract record class BoundNode(BoundNodeKind NodeKind, SyntaxNode Syntax) : INode
{
    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
    {
        BoundNodeWriterExtensions.WriteTo(this, writer);
    }

    public abstract IEnumerable<INode> Descendants();

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}