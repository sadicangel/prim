namespace CodeAnalysis.Syntax;

public interface INode
{
    void PrettyPrint(TextWriter writer, string indent = "", bool isLast = true);

    IEnumerable<INode> GetChildren();
}
