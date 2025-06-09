using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;

internal record class SyntaxIterator(IReadOnlyList<SyntaxToken> Tokens, DiagnosticBag Diagnostics)
{
    private const int MaxSuccessiveMatchTokenErrors = 1;

    private int _successiveMatchTokenErrors = 0;

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
            if (TryMatch(out var token, syntaxKind))
            {
                _successiveMatchTokenErrors = 0;
                return token;
            }
        }

        if (_successiveMatchTokenErrors++ < MaxSuccessiveMatchTokenErrors)
        {
            Diagnostics.ReportUnexpectedToken(syntaxKinds[0], Current);
        }

        var syntheticToken = SyntaxToken.CreateSynthetic(syntaxKinds[0], Current.SyntaxTree, Offset..(Offset + 1));

        // Avoid overflowing the stack.
        ++Offset;

        return syntheticToken;
    }
}
