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

        writer.WriteColored((object)indent, ConsoleColor.DarkGray);
        writer.WriteColored((object)marker, ConsoleColor.DarkGray);
        writer.WriteColored(Name, ConsoleColor.Cyan);
        writer.WriteLine();
    }

    public override string ToString() => Name;
}