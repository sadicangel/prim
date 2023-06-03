namespace CodeAnalysis.Syntax;

public interface INode
{
    TextSpan Span { get; }

    void PrettyPrint(TextWriter writer, string indent = "", bool isLast = true);

    IEnumerable<INode> GetChildren();
}
