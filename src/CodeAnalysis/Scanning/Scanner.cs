using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal static partial class Scanner
{
    internal static Result<ImmutableArray<SyntaxToken>> Scan(SourceText sourceText)
    {
        var diagnostics = new DiagnosticBag();
        var syntaxTokens = ScanImplementation(sourceText, diagnostics).ToImmutableArray();

        return new Result<ImmutableArray<SyntaxToken>>(syntaxTokens, [.. diagnostics]);

        // We probably don't need the deferred execution here.
        static IEnumerable<SyntaxToken> ScanImplementation(SourceText sourceText, DiagnosticBag diagnostics)
        {
            var badTokens = new List<SyntaxToken>();
            var length = 0;
            SyntaxToken token;
            do
            {
                length += ScanSyntaxToken(sourceText, diagnostics, length, out token);
                if (token.SyntaxKind is SyntaxKind.InvalidSyntax)
                {
                    badTokens.Add(token);
                    continue;
                }

                if (badTokens.Count > 0)
                {
                    token = token with
                    {
                        LeadingTrivia = new SyntaxList<SyntaxTrivia>(
                        [
                            ..badTokens.SelectMany(ToInvalidTextTrivia),
                            ..token.LeadingTrivia
                        ])
                    };
                    badTokens.Clear();

                    static IEnumerable<SyntaxTrivia> ToInvalidTextTrivia(SyntaxToken token)
                    {
                        foreach (var trivia in token.LeadingTrivia)
                            yield return trivia;
                        yield return new SyntaxTrivia(SyntaxKind.InvalidTextTrivia, token.SourceSpan);
                        foreach (var trivia in token.TrailingTrivia)
                            yield return trivia;
                    }
                }

                yield return token;
            } while (token.SyntaxKind is not SyntaxKind.EofToken);
        }
    }
}
