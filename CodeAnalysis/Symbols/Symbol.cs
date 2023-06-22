using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Symbols;

public abstract record class Symbol(string Name, SymbolKind Kind) : INode
{
    TextSpan INode.Span { get; }

    IEnumerable<INode> INode.GetChildren() => Enumerable.Empty<INode>();
    void INode.WriteTo(TextWriter writer, string indent, bool isLast)
    {
        var marker = isLast ? "└──" : "├──";

        writer.WriteColored(indent, ConsoleColor.DarkGray);
        writer.WriteColored(marker, ConsoleColor.DarkGray);
        writer.WriteColored(Kind, ConsoleColor.Cyan);
        writer.WriteLine();
    }

    public override string ToString()
    {
        INode node = this;
        using var writer = new StringWriter();
        node.WriteTo(writer);
        return writer.ToString();
    }
}