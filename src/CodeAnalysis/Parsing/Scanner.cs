using CodeAnalysis.Text;
using System.Text;

namespace CodeAnalysis.Syntax.Parsing;

internal static class Scanner
{
    internal static IEnumerable<SyntaxToken> Scan(SyntaxTree syntaxTree)
    {
        var badTokens = new List<SyntaxToken>();
        var position = 0;
        SyntaxToken token;
        do
        {
            position += ScanToken(syntaxTree, position, out token);
            if (token.SyntaxKind is SyntaxKind.InvalidSyntax)
            {
                badTokens.Add(token);
                continue;
            }

            if (badTokens.Count > 0)
            {
                token = token with
                {
                    LeadingTrivia = [
                        ..badTokens.SelectMany(ToInvalidTextTrivia),
                        ..token.LeadingTrivia
                    ]
                };
                badTokens.Clear();

                static IEnumerable<SyntaxTrivia> ToInvalidTextTrivia(SyntaxToken token)
                {
                    foreach (var trivia in token.LeadingTrivia)
                        yield return trivia;
                    yield return new SyntaxTrivia(SyntaxKind.InvalidTextTrivia, token.SyntaxTree, token.Range);
                    foreach (var trivia in token.TrailingTrivia)
                        yield return trivia;
                }
            }
            yield return token;
        }
        while (token.SyntaxKind is not SyntaxKind.EofToken);
    }

    private static int ScanToken(SyntaxTree syntaxTree, int position, out SyntaxToken token)
    {
        var read = 0;
        read += ScanTrivia(syntaxTree, position + read, leading: true, out var leadingTrivia);
        read += ScanTokenOnly(syntaxTree, position + read, out var syntaxKind, out var range, out var value);
        read += ScanTrivia(syntaxTree, position + read, leading: false, out var trailingTrivia);

        token = new SyntaxToken(syntaxKind, syntaxTree, range, leadingTrivia, trailingTrivia, value);
        return read;
    }

    private static int ScanTokenOnly(SyntaxTree syntaxTree, int position, out SyntaxKind kind, out Range range, out object? value)
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

            case ['=', '>', ..]:
                kind = SyntaxKind.LambdaToken;
                range = position..(position + 2);
                value = null;
                return 2;

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
                kind = SyntaxKind.GreaterGreaterEqualsToken;
                range = position..(position + 3);
                value = null;
                return 3;

            case ['>', '>', ..]:
                kind = SyntaxKind.GreaterGreaterToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['>', '=', ..]:
                kind = SyntaxKind.GreaterEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['>', ..]:
                kind = SyntaxKind.GreaterToken;
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
                kind = SyntaxKind.LessLessEqualsToken;
                range = position..(position + 3);
                value = null;
                return 3;

            case ['<', '<', ..]:
                kind = SyntaxKind.LessLessToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['<', '=', ..]:
                kind = SyntaxKind.LessEqualsToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['<', ..]:
                kind = SyntaxKind.LessToken;
                range = position..(position + 1);
                value = null;
                return 1;

            case ['-', '>', ..]:
                kind = SyntaxKind.ArrowToken;
                range = position..(position + 2);
                value = null;
                return 2;

            case ['-', '-', ..]:
                kind = SyntaxKind.MinusMinusToken;
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

            case ['+', '+', ..]:
                kind = SyntaxKind.PlusPlusToken;
                range = position..(position + 2);
                value = null;
                return 2;

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

            //case ['o', 'p', 'e', 'r', 'a', 't', 'o', 'r', var @operator, ..] when IsOperator(@operator):
            //    return ScanOperator(syntaxTree, position, out kind, out range, out value);

            case ['"', ..]:
                return ScanString(syntaxTree, position, out kind, out range, out value);

            case [var d1, ..] when Char.IsAsciiDigit(d1):
            case ['.', var d2, ..] when Char.IsAsciiDigit(d2):
                return ScanNumber(syntaxTree, position, out kind, out range, out value);

            case ['_', ..]:
            case [var l, ..] when Char.IsAsciiLetter(l):
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

    private static int ScanTrivia(SyntaxTree syntaxTree, int position, bool leading, out List<SyntaxTrivia> trivia)
    {
        trivia = [];
        var totalScan = 0;
        while (true)
        {
            var item = default(SyntaxTrivia)!;
            var read = syntaxTree.SourceText[(position + totalScan)..] switch
            {
            ['/', '*', ..] => ScanMultiLineComment(syntaxTree, position, out item),
            ['/', '/', ..] => ScanSingleLineComment(syntaxTree, position, out item),
            ['\n' or '\r', ..] => ScanLineBreak(syntaxTree, position, out item),
            [' ' or '\t', ..] => ScanWhiteSpace(syntaxTree, position, out item),
            [var whitespace, ..] when Char.IsWhiteSpace(whitespace) => ScanWhiteSpace(syntaxTree, position, out item),
                _ => 0
            };

            if (read == 0)
                break;

            totalScan += read;

            if (read > 0)
                trivia.Add(item);

            if (item.SyntaxKind == SyntaxKind.LineBreakTrivia && !leading)
                break;
        }
        return totalScan;
    }

    private static int ScanLineBreak(SyntaxTree syntaxTree, int position, out SyntaxTrivia trivia)
    {
        var read = 0;
        if (syntaxTree.SourceText[(position + read)..] is ['\r', '\n', ..])
            read++;
        read++;

        trivia = new SyntaxTrivia(SyntaxKind.LineBreakTrivia, syntaxTree, position..(position + read));
        return read;
    }

    private static int ScanWhiteSpace(SyntaxTree syntaxTree, int position, out SyntaxTrivia trivia)
    {
        var done = false;
        var read = 0;
        while (!done)
        {
            switch (syntaxTree.SourceText[(position + read)..])
            {
                case []:
                case ['\0' or '\r' or '\n', ..]:
                case [var c, ..] when !Char.IsWhiteSpace(c):
                    done = true;
                    break;
                default:
                    read++;
                    break;
            }
        }

        trivia = new SyntaxTrivia(SyntaxKind.WhiteSpaceTrivia, syntaxTree, position..(position + read));
        return read;
    }

    private static int ScanMultiLineComment(SyntaxTree syntaxTree, int position, out SyntaxTrivia trivia)
    {
        var done = false;
        // Skip '/*'.
        var read = 2;
        while (!done)
        {
            switch (syntaxTree.SourceText[(position + read)..])
            {
                case []:
                case ['\0', ..]:
                    syntaxTree.Diagnostics.ReportUnterminatedComment(new SourceLocation(syntaxTree.SourceText, (position + read)..(position + read + 2)));
                    done = true;
                    break;
                case ['*', '/', ..]:
                    read += 2;
                    done = true;
                    break;
                default:
                    read++;
                    break;
            }
        }

        trivia = new SyntaxTrivia(SyntaxKind.MultiLineCommentTrivia, syntaxTree, position..(position + read));
        return read;
    }

    private static int ScanSingleLineComment(SyntaxTree syntaxTree, int position, out SyntaxTrivia trivia)
    {
        // Skip '//'.
        var read = 2;
        while (syntaxTree.SourceText[position + read] is not '\r' and not '\n' and not '\0')
            read++;

        trivia = new SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, syntaxTree, position..(position + read));
        return read;
    }

    private static int ScanNumber(SyntaxTree syntaxTree, int position, out SyntaxKind kind, out Range range, out object? value)
    {
        var read = 0;
        var isInteger = syntaxTree.SourceText[position + read] != '.';
        do
        {
            read++;
        }
        while (Char.IsDigit(syntaxTree.SourceText[position + read]));

        // Scan the decimal part of a floating point?
        if (isInteger && syntaxTree.SourceText[(position + read)..] is ['.', '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9', ..])
        {
            isInteger = false;
            // We stop when there is no dot after a number. This way we grab all dots in a number (which we report as incorrect).
            while (true)
            {
                do
                {
                    read++;
                }
                while (Char.IsDigit(syntaxTree.SourceText[position + read]));

                // Keep consuming if the next input is a single '.'.
                if (syntaxTree.SourceText[(position + read)..] is not ['.', not '.', ..])
                    break;
            }
        }

        var localCopyOfRange = range = position..(position + read);
        var text = syntaxTree.SourceText[range];
        (kind, value) = isInteger
            ? (SyntaxKind.I32LiteralToken, EnsureCorrectType<int>(text, SyntaxKind.I32LiteralToken))
            : (SyntaxKind.F32LiteralToken, EnsureCorrectType<float>(text, SyntaxKind.F32LiteralToken));

        return read;

        object EnsureCorrectType<T>(ReadOnlySpan<char> text, SyntaxKind kind) where T : unmanaged, ISpanParsable<T>
        {
            if (!T.TryParse(text, provider: null, out T value))
                syntaxTree.Diagnostics.ReportInvalidSyntaxValue(new SourceLocation(syntaxTree.SourceText, localCopyOfRange), text.ToString(), kind);
            return value;
        }
    }

    private static int ScanString(SyntaxTree syntaxTree, int position, out SyntaxKind kind, out Range range, out object? value)
    {
        var builder = new StringBuilder();
        var done = false;
        var read = 1;
        while (!done)
        {
            var span = syntaxTree.SourceText[(position + read)..];
            switch (span)
            {
                case ['\0', ..]:
                case ['\r', ..]:
                case ['\n', ..]:
                case []:
                    syntaxTree.Diagnostics.ReportUnterminatedString(new SourceLocation(syntaxTree.SourceText, position..(position + 1)));
                    done = true;
                    break;
                case ['\\', '"', ..]:
                    read++;
                    builder.Append(span[1]);
                    read++;
                    break;
                case ['"', ..]:
                    read++;
                    done = true;
                    break;
                default:
                    builder.Append(span[0]);
                    read++;
                    break;
            }
        }

        kind = SyntaxKind.StrLiteralToken;
        range = position..(position + read);
        value = builder.ToString();
        return read;
    }

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

        static bool IsValid(char c) => c is '_' || Char.IsAsciiLetterOrDigit(c);
    }
}
