using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis;

public interface INode
{
    TextSpan Span { get; }

    void WriteTo(TextWriter writer, string indent = "", bool isLast = true);

    IEnumerable<INode> GetChildren();

    public Token GetLastToken()
    {
        if (this is Token token)
            return token;

        return GetChildren().Last().GetLastToken();
    }
}
