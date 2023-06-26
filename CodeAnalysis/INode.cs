namespace CodeAnalysis;

public interface INode
{
    void WriteTo(TextWriter writer, string indent = "", bool isLast = true);

    IEnumerable<INode> GetChildren();
}
