using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binding;

internal abstract record class BoundOperator : INode
{
    TextSpan INode.Span { get; }

    public IEnumerable<INode> GetChildren() => Enumerable.Empty<INode>();

    protected abstract string GetDisplayString();

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        writer.WriteColored(indent, ConsoleColor.DarkGray);
        writer.WriteColored(marker, ConsoleColor.DarkGray);
        writer.WriteColored(GetDisplayString(), ConsoleColor.Cyan);
        writer.WriteLine();
    }

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}
