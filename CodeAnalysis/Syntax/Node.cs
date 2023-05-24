namespace CodeAnalysis.Syntax;

public abstract record class Node(NodeKind Kind) : IPrintableNode
{
    public void PrettyPrint(TextWriter writer, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        writer.Write(indent);
        writer.Write(marker);
        writer.Write(Kind);
        writer.WriteLine();

        indent += isLast ? "   " : "│  ";

        var lastChild = GetChildren().LastOrDefault();

        foreach (var child in GetChildren())
            child.PrettyPrint(writer, indent, child == lastChild);
    }

    public abstract IEnumerable<IPrintableNode> GetChildren();
}
