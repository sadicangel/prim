using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Scanning;

internal partial class Scanner
{
    private static int ScanSyntaxKind(SourceText sourceText, DiagnosticBag diagnostics, int offset, out SyntaxKind kind, out Range range, out object? value)
    {
        switch (sourceText[offset..])
        {
            // Punctuation
            case ['{', ..]:
                kind = SyntaxKind.BraceOpenToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['}', ..]:
                kind = SyntaxKind.BraceCloseToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['(', ..]:
                kind = SyntaxKind.ParenthesisOpenToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case [')', ..]:
                kind = SyntaxKind.ParenthesisCloseToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['[', ..]:
                kind = SyntaxKind.BracketOpenToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case [']', ..]:
                kind = SyntaxKind.BracketCloseToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case [':', ':', ..]:
                kind = SyntaxKind.ColonColonToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case [':', ..]:
                kind = SyntaxKind.ColonToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case [';', ..]:
                kind = SyntaxKind.SemicolonToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case [',', ..]:
                kind = SyntaxKind.CommaToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['=', '>', ..]:
                kind = SyntaxKind.EqualsGreaterThanToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['-', '>', ..]:
                kind = SyntaxKind.MinusGreaterThanToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            // Operators
            case ['&', '&', ..]:
                kind = SyntaxKind.AmpersandAmpersandToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['&', '=', ..]:
                kind = SyntaxKind.AmpersandEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['&', ..]:
                kind = SyntaxKind.AmpersandToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['!', '=', ..]:
                kind = SyntaxKind.ExclamationEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['!', ..]:
                kind = SyntaxKind.ExclamationToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['.', '.', ..]:
                kind = SyntaxKind.DotDotToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['.']:
            case ['.', not ('0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9'), ..]:
                kind = SyntaxKind.DotToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['=', '=', ..]:
                kind = SyntaxKind.EqualsEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['=', ..]:
                kind = SyntaxKind.EqualsToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['>', '>', '=', ..]:
                kind = SyntaxKind.GreaterThanGreaterThanEqualsToken;
                range = offset..(offset + 3);
                value = null;
                return 3;

            case ['>', '>', ..]:
                kind = SyntaxKind.GreaterThanGreaterThanToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['>', '=', ..]:
                kind = SyntaxKind.GreaterThanEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['>', ..]:
                kind = SyntaxKind.GreaterThanToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['^', '=', ..]:
                kind = SyntaxKind.CaretEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['^', ..]:
                kind = SyntaxKind.CaretToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['?', '?', '=', ..]:
                kind = SyntaxKind.HookHookEqualsToken;
                range = offset..(offset + 3);
                value = null;
                return 3;

            case ['?', '?', ..]:
                kind = SyntaxKind.HookHookToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['?', ..]:
                kind = SyntaxKind.HookToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['<', '<', '=', ..]:
                kind = SyntaxKind.LessThanLessThanEqualsToken;
                range = offset..(offset + 3);
                value = null;
                return 3;

            case ['<', '<', ..]:
                kind = SyntaxKind.LessThanLessThanToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['<', '=', ..]:
                kind = SyntaxKind.LessThanEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['<', ..]:
                kind = SyntaxKind.LessThanToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['-', '=', ..]:
                kind = SyntaxKind.MinusEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['-', ..]:
                kind = SyntaxKind.MinusToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['%', '=', ..]:
                kind = SyntaxKind.PercentEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['%', ..]:
                kind = SyntaxKind.PercentToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['|', '|', ..]:
                kind = SyntaxKind.BarBarToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['|', '=', ..]:
                kind = SyntaxKind.PipeEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['|', ..]:
                kind = SyntaxKind.BarToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['+', '=', ..]:
                kind = SyntaxKind.PlusEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['+', ..]:
                kind = SyntaxKind.PlusToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['/', '=', ..]:
                kind = SyntaxKind.SlashEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['/', ..]:
                kind = SyntaxKind.SlashToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['*', '*', '=', ..]:
                kind = SyntaxKind.StarStarEqualsToken;
                range = offset..(offset + 3);
                value = null;
                return 3;

            case ['*', '*', ..]:
                kind = SyntaxKind.AsteriskAsteriskToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['*', '=', ..]:
                kind = SyntaxKind.StarEqualsToken;
                range = offset..(offset + 2);
                value = null;
                return 2;

            case ['*', ..]:
                kind = SyntaxKind.AsteriskToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['~', ..]:
                kind = SyntaxKind.TildeToken;
                range = offset..(offset + 1);
                value = null;
                return 1;

            case ['"', ..]:
                return ScanString(sourceText, diagnostics, offset, out kind, out range, out value);

            case [var d1, ..] when char.IsAsciiDigit(d1):
            case ['.', var d2, ..] when char.IsAsciiDigit(d2):
                return ScanNumber(sourceText, diagnostics, offset, out kind, out range, out value);

            case ['_', ..]:
            case [var l, ..] when char.IsAsciiLetter(l):
                return ScanIdentifier(sourceText, offset, out kind, out range, out value);

            // Control
            case []:
                kind = SyntaxKind.EofToken;
                range = offset..offset;
                value = null;
                return 0;

            default:
                diagnostics.ReportInvalidCharacter(new SourceSpan(sourceText, offset..(offset + 1)), sourceText[offset]);
                kind = SyntaxKind.InvalidSyntax;
                range = offset..(offset + 1);
                value = null;
                return 1;
        }
    }
}
