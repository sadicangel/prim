using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis;

public sealed record class Variable(string Name, bool IsReadOnly, Type Type) : INode
{
    TextSpan INode.Span { get; }

    public bool Equals(Variable? other) => other?.Name == Name;
    public override int GetHashCode() => Name.GetHashCode();
    public IEnumerable<INode> GetChildren() => Enumerable.Empty<INode>();
    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        writer.WriteColored(indent, ConsoleColor.DarkGray);
        writer.WriteColored(marker, ConsoleColor.DarkGray);
        writer.WriteColored(nameof(Variable), ConsoleColor.Cyan);
        writer.WriteLine();
    }

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}