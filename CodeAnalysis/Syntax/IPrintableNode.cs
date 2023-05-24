namespace CodeAnalysis.Syntax;

public interface IPrintableNode
{
    void PrettyPrint(TextWriter writer, string indent = "", bool isLast = true);

    IEnumerable<IPrintableNode> GetChildren();
}
