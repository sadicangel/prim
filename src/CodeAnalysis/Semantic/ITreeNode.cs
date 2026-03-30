namespace CodeAnalysis.Semantic;

internal interface ITreeNode
{
    IEnumerable<ITreeNode> Children();
}
