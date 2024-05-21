using CodeAnalysis.Syntax;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Parsing;

internal record class SyntaxTokenIterator(IReadOnlyList<SyntaxToken> Tokens)
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

    public SyntaxToken Next()
    {
        var current = Current;
        ++Index;
        return current;
    }

    public SyntaxToken Match(SyntaxKind syntaxKind)
    {
        if (!TryMatch(syntaxKind, out var token))
        {
            if (_successiveMatchTokenErrors++ < MaxSuccessiveMatchTokenErrors)
                Current.SyntaxTree.Diagnostics.ReportUnexpectedToken(syntaxKind, Current);

            return new SyntaxToken(
                syntaxKind,
                Current.SyntaxTree,
                Current.Range,
                SyntaxFactory.EmptyTrivia(),
                SyntaxFactory.EmptyTrivia(),
                string.Empty);
        }
        _successiveMatchTokenErrors = 0;
        return token;
    }

    public bool TryMatch(SyntaxKind syntaxKind, [MaybeNullWhen(false)] out SyntaxToken token) => (token = MatchOrDefault(syntaxKind)) is not null;

    public SyntaxToken? MatchOrDefault(SyntaxKind syntaxKind) => Current.SyntaxKind == syntaxKind ? Next() : null;

    public SyntaxToken Match(params ReadOnlySpan<SyntaxKind> syntaxKinds)
    {
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
