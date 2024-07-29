using CodeAnalysis.Syntax;

namespace CodeAnalysis.Scanning;
partial class Lexer
{
    private static int ScanIdentifier(SyntaxTree syntaxTree, int position, out SyntaxKind kind, out Range range, out object? value)
    {
        var read = 0;
        do
        {
            read++;
        }
        while (IsValid(syntaxTree.SourceText[position + read]));

        range = position..(position + read);
        var text = syntaxTree.SourceText[range];
        kind = SyntaxFacts.GetKeywordKind(text);
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
