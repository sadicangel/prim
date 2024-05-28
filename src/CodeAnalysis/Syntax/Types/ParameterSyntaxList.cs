
namespace CodeAnalysis.Syntax.Types;

public sealed record class ParameterSyntaxList(SyntaxTree SyntaxTree, SeparatedSyntaxList<ParameterSyntax> Parameters)
    : SyntaxNode(SyntaxKind.ParameterList, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        foreach (var node in Parameters.SyntaxNodes)
            yield return node;
    }
}
