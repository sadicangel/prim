namespace CodeAnalysis;
public interface INode
{
    IEnumerable<INode> Children();
}
