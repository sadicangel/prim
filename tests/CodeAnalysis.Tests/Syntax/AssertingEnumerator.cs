using CodeAnalysis.Syntax;

namespace CodeAnalysis.Tests.Syntax;

internal sealed class AssertingEnumerator : IDisposable
{
    private readonly IEnumerator<INode> _enumerator;
    private bool _hasErrors;

    public AssertingEnumerator(INode node)
    {
        _enumerator = Flatten(node).GetEnumerator();

        static IEnumerable<INode> Flatten(INode token)
        {
            var stack = new Stack<INode>();
            stack.Push(token);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;

                foreach (var child in current.Children().Reverse())
                    stack.Push(child);
            }
        }
    }

    private bool MarkFailed()
    {
        _hasErrors = true;
        return false;
    }

    public void Dispose()
    {
        if (!_hasErrors)
            Assert.False(_enumerator.MoveNext());
        _enumerator.Dispose();
    }

    public void AssertNode(SyntaxNodeKind nodeKind)
    {
        try
        {
            Assert.True(_enumerator.MoveNext());
            var node = Assert.IsAssignableFrom<SyntaxNode>(_enumerator.Current);
            Assert.Equal(nodeKind, node.NodeKind);
        }
        catch when (MarkFailed())
        {
            throw;
        }
    }

    public void AssertToken(TokenKind kind, string text)
    {
        try
        {
            Assert.True(_enumerator.MoveNext());
            var token = Assert.IsType<Token>(_enumerator.Current);
            Assert.Equal(kind, token.TokenKind);
            Assert.Equal(text, token.Text);
        }
        catch when (MarkFailed())
        {
            throw;
        }
    }
}