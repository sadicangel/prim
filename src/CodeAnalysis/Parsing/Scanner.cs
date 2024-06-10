using System.Globalization;
using System.Text;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Parsing;

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
                    LeadingTrivia = new([
                        ..badTokens.SelectMany(ToInvalidTextTrivia),
                        ..token.LeadingTrivia
                    ])
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

            //case ['o', 'p', 'e', 'r', 'a', 't', 'o', 'r', var @operator, ..] when IsOperator(@operator):
            //    return ScanOperator(syntaxTree, position, out kind, out range, out value);

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

    private static int ScanTrivia(SyntaxTree syntaxTree, int position, bool leading, out SyntaxList<SyntaxTrivia> trivia)
    {
        var builder = new SyntaxList<SyntaxTrivia>.Builder();
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
            [var whitespace, ..] when char.IsWhiteSpace(whitespace) => ScanWhiteSpace(syntaxTree, position, out item),
                _ => 0
            };

            if (read == 0)
                break;

            totalScan += read;

            if (read > 0)
                builder.Add(item);

            if (item.SyntaxKind == SyntaxKind.LineBreakTrivia && !leading)
                break;
        }
        trivia = builder.IsDefault ? SyntaxFactory.EmptyTrivia() : builder.ToSyntaxList();
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
                case [var c, ..] when !char.IsWhiteSpace(c):
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
        var isFloat = false;
        var isInvalid = false;
        var numberStyles = NumberStyles.Number;

        // TODO: Actually handle this case.
        if (syntaxTree.SourceText[position + read] is '-')
            read++;

        switch (syntaxTree.SourceText[(position + read)..])
        {
            case ['0', 'b', ..]:
                {
                    read += 2;
                    while (syntaxTree.SourceText[position + read] is '0' or '1')
                    {
                        ++read;
                    }
                    numberStyles = NumberStyles.BinaryNumber;
                }
                break;
            //case ['0', '0', ..]: Octal
            case ['0', 'x' or 'X', ..]:
                {
                    read += 2;
                    while (char.IsAsciiHexDigit(syntaxTree.SourceText[position + read]))
                    {
                        ++read;
                    }
                    numberStyles = NumberStyles.HexNumber;
                }
                break;
            default:
                {
                    while (char.IsAsciiDigit(syntaxTree.SourceText[position + read]))
                    {
                        ++read;
                    }

                    if (syntaxTree.SourceText[position + read] is '.')
                    {
                        isFloat = true;
                        ++read;
                        while (char.IsAsciiDigit(syntaxTree.SourceText[position + read]))
                        {
                            ++read;
                        }
                    }

                    if (syntaxTree.SourceText[position + read] is 'e' or 'E')
                    {
                        isFloat = true;
                        if (!char.IsAsciiDigit(syntaxTree.SourceText[position + read - 1]))
                        {
                            isInvalid = true;
                        }
                        ++read;
                        while (char.IsAsciiDigit(syntaxTree.SourceText[position + read]))
                        {
                            ++read;
                        }
                    }
                    break;
                }
        }

        if (isFloat)
        {
            kind = SyntaxKind.F64LiteralToken;
            value = 0D;

            if (!isInvalid)
            {
                switch (syntaxTree.SourceText[position + read])
                {
                    case 'f' or 'F':
                        read++;
                        kind = SyntaxKind.F32LiteralToken;
                        isInvalid = !float.TryParse(syntaxTree.SourceText[position..(position + read - 1)], out var f32);
                        value = f32;
                        break;

                    case 'd' or 'D':
                        read++;
                        kind = SyntaxKind.F64LiteralToken;
                        isInvalid = !double.TryParse(syntaxTree.SourceText[position..(position + read - 1)], out var f64);
                        value = f64;
                        break;

                    default:
                        kind = SyntaxKind.F64LiteralToken;
                        isInvalid = !double.TryParse(syntaxTree.SourceText[position..(position + read - 1)], out var @float);
                        value = @float;
                        break;
                }
            }
        }
        else
        {
            switch (syntaxTree.SourceText[(position + read)..])
            {
                case ['l' or 'L', ..]:
                    read++;
                    kind = SyntaxKind.I64LiteralToken;
                    isInvalid = !long.TryParse(syntaxTree.SourceText[position..(position + read - 1)], numberStyles, CultureInfo.InvariantCulture, out var i64);
                    value = i64;
                    break;

                case ['u' or 'U', 'l' or 'L', ..]:
                    read += 2;
                    kind = SyntaxKind.U64LiteralToken;
                    isInvalid = !ulong.TryParse(syntaxTree.SourceText[position..(position + read - 2)], numberStyles, CultureInfo.InvariantCulture, out var u64);
                    value = u64;
                    break;

                case ['u' or 'U', ..]:
                    read++;
                    kind = SyntaxKind.U32LiteralToken;
                    isInvalid = !uint.TryParse(syntaxTree.SourceText[position..(position + read - 1)], numberStyles, CultureInfo.InvariantCulture, out var u32);
                    value = u32;
                    break;

                case ['f' or 'F', ..]:
                    read++;
                    kind = SyntaxKind.F32LiteralToken;
                    isInvalid = !float.TryParse(syntaxTree.SourceText[position..(position + read - 1)], CultureInfo.InvariantCulture, out var f32);
                    value = f32;
                    break;

                case ['d' or 'D', ..]:
                    read++;
                    kind = SyntaxKind.F64LiteralToken;
                    isInvalid = !double.TryParse(syntaxTree.SourceText[position..(position + read - 1)], CultureInfo.InvariantCulture, out var f64);
                    value = f64;
                    break;

                default:
                    kind = SyntaxKind.I32LiteralToken;
                    value = 0;
                    isInvalid = true;
                    if (long.TryParse(syntaxTree.SourceText[position..(position + read)], numberStyles, CultureInfo.InvariantCulture, out var @int))
                    {
                        isInvalid = false;
                        if (@int is >= int.MinValue and <= int.MaxValue)
                        {
                            kind = SyntaxKind.I32LiteralToken;
                            value = (int)@int;
                        }
                        else
                        {
                            kind = SyntaxKind.I64LiteralToken;
                            value = @int;
                        }
                    }
                    break;
            }
        }

        range = position..(position + read);
        if (isInvalid)
        {
            syntaxTree.Diagnostics.ReportInvalidSyntaxValue(
                new SourceLocation(syntaxTree.SourceText, range),
                kind);
        }

        return read;
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

        static bool IsValid(char c) => c is '_' || char.IsAsciiLetterOrDigit(c);
    }
}
