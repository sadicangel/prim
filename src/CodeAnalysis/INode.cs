using Spectre.Console;

namespace CodeAnalysis;
public interface INode
{
    void WriteTo(TreeNode parent);

    IEnumerable<INode> Children();
}
