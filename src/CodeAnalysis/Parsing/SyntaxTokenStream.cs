using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Parsing;

internal record class SyntaxTokenStream(SourceText SourceText, IReadOnlyList<SyntaxToken> Tokens)
{
    private const int MaxSuccessiveMatchTokenErrors = 1;

    private int _successiveMatchTokenErrors = 0;

    public DiagnosticBag Diagnostics { get; } = [];

    public int Offset { get; private set; }

    public SyntaxToken Current { get => Tokens[int.Clamp(Offset, 0, Tokens.Count - 1)]; }

    public SyntaxToken Peek(int offset = 0)
    {
        var index = Offset + offset;
        if (index >= Tokens.Count)
            return Tokens[^1];
        return Tokens[index];
    }

    public bool TryMatch([MaybeNullWhen(false)] out SyntaxToken token, params ReadOnlySpan<SyntaxKind> syntaxKinds)
    {
        if (syntaxKinds.Length == 0)
        {
            token = Current;
            ++Offset;
            return true;
        }

        foreach (var syntaxKind in syntaxKinds)
        {
            if (syntaxKind == Current.SyntaxKind)
            {
                token = Current;
                ++Offset;
                return true;
            }
        }

        token = null;
        return false;
    }

    public SyntaxToken Match(params ReadOnlySpan<SyntaxKind> syntaxKinds)
    {
        if (syntaxKinds.Length == 0)
        {
            var current = Current;
            ++Offset;
            return current;
        }

        foreach (var syntaxKind in syntaxKinds)
        {
            if (!TryMatch(out var token, syntaxKind))
            {
                continue;
            }

            _successiveMatchTokenErrors = 0;
            return token;
        }

        if (_successiveMatchTokenErrors++ < MaxSuccessiveMatchTokenErrors)
        {
            Diagnostics.ReportUnexpectedToken(syntaxKinds[0], Current);
        }

        var syntheticToken = SyntaxToken.CreateSynthetic(syntaxKinds[0]);

        // Avoid overflowing the stack.
        ++Offset;

        return syntheticToken;
    }

    public bool Skip(int count = 1)
    {
        var offset = Offset + count;
        if (offset >= Tokens.Count) return false;
        Offset = offset;
        return true;
    }

    public bool SkipUntil(SyntaxKind syntaxKind)
    {
        while (Current.SyntaxKind != syntaxKind)
        {
            if (!Skip()) return false;
        }

        return true;
    }

    public bool IsLambdaExpressionAhead()
    {
        using var checkpoint = new Checkpoint(this);
        if (Current.SyntaxKind == SyntaxKind.ParenthesisOpenToken && SkipUntil(SyntaxKind.ParenthesisCloseToken))
        {
            Skip();
            return Current.SyntaxKind == SyntaxKind.EqualsGreaterThanToken;
        }

        return false;
    }

    private readonly ref struct Checkpoint(SyntaxTokenStream stream) : IDisposable
    {
        private readonly int _offset = stream.Offset;

        public void Dispose()
        {
            stream.Offset = _offset;
        }
    }
}
