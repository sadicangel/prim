using CodeAnalysis.Syntax;

namespace CodeAnalysis;

public interface INode
{
    void WriteTo(TextWriter writer, string indent = "", bool isLast = true);

    IEnumerable<INode> GetChildren();

    public Token GetLastToken()
    {
        if (this is Token token)
            return token;

        return GetChildren().Last().GetLastToken();
    }
}
