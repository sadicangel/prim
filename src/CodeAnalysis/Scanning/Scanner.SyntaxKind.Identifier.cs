using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal partial class Scanner
{
    private static int ScanIdentifier(SourceText sourceText, int position, out SyntaxKind kind, out Range range, out object? value)
    {
        var read = 0;
        do
        {
            read++;
        } while (IsValid(sourceText[position + read]));

        range = position..(position + read);
        kind = SyntaxFacts.GetKeywordKind(sourceText[range]);
        value = kind switch
        {
            SyntaxKind.TrueKeyword => true,
            SyntaxKind.FalseKeyword => false,
            _ => null,
        };
        return read;

        static bool IsValid(char c) => c is '_' || char.IsAsciiLetterOrDigit(c);
    }
}
