using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;
partial class Lexer
{
    private static int ScanSyntaxKind(SyntaxTree syntaxTree, int position, out SyntaxKind kind, out Range range, out object? value)
    {
        switch (syntaxTree.SourceText[position..])
        {
            // Punctuation
            case ['{', ..]:
                kind = SyntaxKind.BraceOpenToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['}', ..]:
                kind = SyntaxKind.BraceCloseToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['(', ..]:
                kind = SyntaxKind.ParenthesisOpenToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case [')', ..]:
                kind = SyntaxKind.ParenthesisCloseToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['[', ..]:
                kind = SyntaxKind.BracketOpenToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case [']', ..]:
                kind = SyntaxKind.BracketCloseToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case [':', ..]:
                kind = SyntaxKind.ColonToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case [';', ..]:
                kind = SyntaxKind.SemicolonToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case [',', ..]:
                kind = SyntaxKind.CommaToken;
                range = position..(position + 1);
                value = null;
                return 1;

            // Operators
            case ['&', '&', ..]:
                kind = SyntaxKind.AmpersandAmpersandToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['&', '=', ..]:
                kind = SyntaxKind.AmpersandEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['&', ..]:
                kind = SyntaxKind.AmpersandToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['!', '=', ..]:
                kind = SyntaxKind.BangEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['!', ..]:
                kind = SyntaxKind.BangToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['.', '.', ..]:
                kind = SyntaxKind.DotDotToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['.']:
            case ['.', not ('0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9'), ..]:
                kind = SyntaxKind.DotToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['=', '=', ..]:
                kind = SyntaxKind.EqualsEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['=', ..]:
                kind = SyntaxKind.EqualsToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['>', '>', '=', ..]:
                kind = SyntaxKind.GreaterThanGreaterThanEqualsToken;
                range = position..(position + 3);
                value = null;
                return 3;

            case ['>', '>', ..]:
                kind = SyntaxKind.GreaterThanGreaterThanToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['>', '=', ..]:
                kind = SyntaxKind.GreaterThanEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['>', ..]:
                kind = SyntaxKind.GreaterThanToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['^', '=', ..]:
                kind = SyntaxKind.HatEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['^', ..]:
                kind = SyntaxKind.HatToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['?', '?', '=', ..]:
                kind = SyntaxKind.HookHookEqualsToken;
                range = position..(position + 3);
                value = null;
                return 3;

            case ['?', '?', ..]:
                kind = SyntaxKind.HookHookToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['?', ..]:
                kind = SyntaxKind.HookToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['<', '<', '=', ..]:
                kind = SyntaxKind.LessThanLessThanEqualsToken;
                range = position..(position + 3);
                value = null;
                return 3;

            case ['<', '<', ..]:
                kind = SyntaxKind.LessThanLessThanToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['<', '=', ..]:
                kind = SyntaxKind.LessThanEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['<', ..]:
                kind = SyntaxKind.LessThanToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['-', '>', ..]:
                kind = SyntaxKind.MinusGreaterThanToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['-', '=', ..]:
                kind = SyntaxKind.MinusEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['-', ..]:
                kind = SyntaxKind.MinusToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['%', '=', ..]:
                kind = SyntaxKind.PercentEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['%', ..]:
                kind = SyntaxKind.PercentToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['|', '|', ..]:
                kind = SyntaxKind.PipePipeToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['|', '=', ..]:
                kind = SyntaxKind.PipeEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['|', ..]:
                kind = SyntaxKind.PipeToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['+', '=', ..]:
                kind = SyntaxKind.PlusEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['+', ..]:
                kind = SyntaxKind.PlusToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['/', '=', ..]:
                kind = SyntaxKind.SlashEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['/', ..]:
                kind = SyntaxKind.SlashToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['*', '*', '=', ..]:
                kind = SyntaxKind.StarStarEqualsToken;
                range = position..(position + 3);
                value = null;
                return 3;

            case ['*', '*', ..]:
                kind = SyntaxKind.StarStarToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['*', '=', ..]:
                kind = SyntaxKind.StarEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['*', ..]:
                kind = SyntaxKind.StarToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['~', ..]:
                kind = SyntaxKind.TildeToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['"', ..]:
                return ScanString(syntaxTree, position, out kind, out range, out value);

            case [var d1, ..] when char.IsAsciiDigit(d1):
            case ['.', var d2, ..] when char.IsAsciiDigit(d2):
                return ScanNumber(syntaxTree, position, out kind, out range, out value);

            case ['_', ..]:
            case [var l, ..] when char.IsAsciiLetter(l):
                return ScanIdentifier(syntaxTree, position, out kind, out range, out value);

            // Control
            case []:
                kind = SyntaxKind.EofToken;
                range = position..position;
                value = null;
                return 0;

            default:
                syntaxTree.Diagnostics.ReportInvalidCharacter(new SourceLocation(syntaxTree.SourceText, position..(position + 1)), syntaxTree.SourceText[position]);
                kind = SyntaxKind.InvalidSyntax;
                range = position..(position + 1);
                value = null;
                return 1;
        }
    }
}
