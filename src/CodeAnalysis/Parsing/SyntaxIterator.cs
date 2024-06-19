using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;

internal record class SyntaxIterator(IReadOnlyList<SyntaxToken> Tokens)
{
    private const int MaxSuccessiveMatchTokenErrors = 1;

    private int _successiveMatchTokenErrors = 0;

    public int Index { get; private set; }

    public SyntaxToken Current { get => Tokens[int.Clamp(Index, 0, Tokens.Count - 1)]; }

    public SyntaxToken Peek(int offset = 0)
    {
        var index = Index + offset;
        if (index >= Tokens.Count)
            return Tokens[^1];
        return Tokens[index];
    }

    public bool TryMatch(SyntaxKind syntaxKind, [MaybeNullWhen(false)] out SyntaxToken token)
    {
        if (syntaxKind == Current.SyntaxKind)
        {
            token = Current;
            ++Index;
            return true;
        }
        token = null;
        return false;
    }

    public SyntaxToken Match(params ReadOnlySpan<SyntaxKind> syntaxKinds)
    {
        if (syntaxKinds.Length == 0)
        {
            var current = Current;
            ++Index;
            return current;
        }

        foreach (var syntaxKind in syntaxKinds)
        {
            if (TryMatch(syntaxKind, out var token))
            {
                _successiveMatchTokenErrors = 0;
                return token;
            }
        }

        if (_successiveMatchTokenErrors++ < MaxSuccessiveMatchTokenErrors)
            Current.SyntaxTree.Diagnostics.ReportUnexpectedToken(syntaxKinds[0], Current);

        return new SyntaxToken(
            syntaxKinds[0],
            Current.SyntaxTree,
            Current.Range,
            SyntaxFactory.EmptyTrivia(),
            SyntaxFactory.EmptyTrivia(),
            string.Empty);
    }
}
