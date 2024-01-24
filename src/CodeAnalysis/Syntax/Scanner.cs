using CodeAnalysis.Text;
using System.Text;

namespace CodeAnalysis.Syntax;

internal static class Scanner
{
    public static IEnumerable<Token> Scan(SyntaxTree syntaxTree)
    {
        var badTokens = new List<Token>();
        var position = 0;
        Token token;
        do
        {
            position += ReadToken(syntaxTree, position, out token);
            if (token.TokenKind is TokenKind.Invalid)
            {
                badTokens.Add(token);
                continue;
            }

            if (badTokens.Count > 0)
            {
                var leadingTrivia = new List<Trivia>();
                for (int i = badTokens.Count - 1; i >= 0; --i)
                {
                    var badToken = badTokens[i];
                    token.Trivia.Leading.InsertRange(0,
                    [
                        .. badToken.Trivia.Leading,
                        new Trivia(syntaxTree, TokenKind.InvalidText, badToken.Range),
                        .. badToken.Trivia.Trailing
                    ]);
                }

                badTokens.Clear();
            }
            yield return token;
        }
        while (token.TokenKind is not TokenKind.EOF);
    }

    private static int ReadToken(SyntaxTree syntaxTree, int position, out Token token)
    {
        position += ReadTrivia(syntaxTree, position, leading: true, out var leadingTrivia);
        position += ReadTokenOnly(syntaxTree, position, out var kind, out var range, out var value);
        position += ReadTrivia(syntaxTree, position, leading: false, out var trailingTrivia);

        token = new Token(syntaxTree, kind, range, new TokenTrivia(leadingTrivia, trailingTrivia), value);
        return position;


        static int ReadTokenOnly(SyntaxTree syntaxTree, int position, out TokenKind kind, out Range range, out object? value)
        {
            switch (syntaxTree.Source[position..])
            {
                // Punctuation
                case ['=', '>', ..]:
                    kind = TokenKind.Arrow;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['{', ..]:
                    kind = TokenKind.OpenBrace;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['(', ..]:
                    kind = TokenKind.OpenParenthesis;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['}', ..]:
                    kind = TokenKind.CloseBrace;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case [')', ..]:
                    kind = TokenKind.CloseParenthesis;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case [':', ..]:
                    kind = TokenKind.Colon;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case [';', ..]:
                    kind = TokenKind.Semicolon;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case [',', ..]:
                    kind = TokenKind.Comma;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                // Operators
                case ['&', '&', ..]:
                    kind = TokenKind.AmpersandAmpersand;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['&', '=', ..]:
                    kind = TokenKind.AmpersandEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['&', ..]:
                    kind = TokenKind.Ampersand;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['!', '=', ..]:
                    kind = TokenKind.BangEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['!', ..]:
                    kind = TokenKind.Bang;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['=', '=', ..]:
                    kind = TokenKind.EqualEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['=', ..]:
                    kind = TokenKind.Equal;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['>', '=', ..]:
                    kind = TokenKind.GreaterEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['>', ..]:
                    kind = TokenKind.Greater;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['^', '=', ..]:
                    kind = TokenKind.HatEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['^', ..]:
                    kind = TokenKind.Hat;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['<', '=', ..]:
                    kind = TokenKind.LessEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['<', ..]:
                    kind = TokenKind.Less;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['-', '=', ..]:
                    kind = TokenKind.MinusEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['-', ..]:
                    kind = TokenKind.Minus;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['%', '=', ..]:
                    kind = TokenKind.PercentEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['%', ..]:
                    kind = TokenKind.Percent;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['|', '=', ..]:
                    kind = TokenKind.PipeEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['|', '|', ..]:
                    kind = TokenKind.PipePipe;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['|', ..]:
                    kind = TokenKind.Pipe;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['+', '=', ..]:
                    kind = TokenKind.PlusEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['+', ..]:
                    kind = TokenKind.Plus;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['.', '.', ..]:
                    kind = TokenKind.Range;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['/', '=', ..]:
                    kind = TokenKind.SlashEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['/', ..]:
                    kind = TokenKind.Slash;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['*', '*', '=', ..]:
                    kind = TokenKind.StarStarEqual;
                    range = position..(position + 3);
                    value = null;
                    return 3;

                case ['*', '=', ..]:
                    kind = TokenKind.StarEqual;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['*', '*', ..]:
                    kind = TokenKind.StarStar;
                    range = position..(position + 2);
                    value = null;
                    return 2;

                case ['*', ..]:
                    kind = TokenKind.Star;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['~', ..]:
                    kind = TokenKind.Tilde;
                    range = position..(position + 1);
                    value = null;
                    return 1;

                case ['"', ..]:
                    return ReadStringToken(syntaxTree, position, out kind, out range, out value);

                case ['0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9', ..]:
                case ['.', '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9', ..]:
                    return ReadNumberToken(syntaxTree, position, out kind, out range, out value);

                case ['A' or 'B' or 'C' or 'D' or 'E' or 'F' or 'G' or 'H' or 'I' or 'J' or 'K' or 'L' or 'M' or 'N' or 'O' or 'P' or 'Q' or 'R' or 'S' or 'T' or 'U' or 'V' or 'W' or 'X' or 'Y' or 'Z' or
                     'a' or 'b' or 'c' or 'd' or 'e' or 'f' or 'g' or 'h' or 'i' or 'j' or 'k' or 'l' or 'm' or 'n' or 'o' or 'p' or 'q' or 'r' or 's' or 't' or 'u' or 'v' or 'w' or 'x' or 'y' or 'z', ..]:
                    return ReadIdentifier(syntaxTree, position, out kind, out range, out value);

                // Control
                case []:
                    kind = TokenKind.EOF;
                    range = position..position;
                    value = null;
                    return 0;

                default:
                    syntaxTree.Diagnostics.ReportInvalidCharacter(new SourceLocation(syntaxTree.Source, position..(position + 1)), syntaxTree.Source[position]);
                    kind = TokenKind.Invalid;
                    range = position..(position + 1);
                    value = null;
                    return 1;
            }
        }

        static int ReadTrivia(SyntaxTree syntaxTree, int position, bool leading, out List<Trivia> trivia)
        {
            trivia = [];
            var totalRead = 0;
            while (true)
            {
                var item = default(Trivia)!;
                var read = syntaxTree.Source[position..] switch
                {
                ['/', '*', ..] => ReadMultiLineComment(syntaxTree, position, out item),
                ['/', '/', ..] => ReadSingleLineComment(syntaxTree, position, out item),
                ['\n' or '\r', ..] => ReadLineBreak(syntaxTree, position, out item),
                [' ' or '\t', ..] => ReadWhiteSpace(syntaxTree, position, out item),
                [var whitespace, ..] when Char.IsWhiteSpace(whitespace) => ReadWhiteSpace(syntaxTree, position, out item),
                    _ => 0
                };

                if (read == 0)
                    break;

                if (position > read)
                    trivia.Add(item);

                if (item.TokenKind == TokenKind.LineBreak && !leading)
                    break;
            }
            return totalRead;
        }

        static int ReadLineBreak(SyntaxTree syntaxTree, int position, out Trivia trivia)
        {
            var read = 0;
            if (syntaxTree.Source[(position + read)..] is ['\r', '\n', ..])
                read++;
            read++;

            trivia = new Trivia(syntaxTree, TokenKind.LineBreak, position..(position + read));
            return read;
        }

        static int ReadWhiteSpace(SyntaxTree syntaxTree, int position, out Trivia trivia)
        {
            var done = false;
            var read = 0;
            while (!done)
            {
                switch (syntaxTree.Source[(position + read)..])
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

            trivia = new Trivia(syntaxTree, TokenKind.WhiteSpace, position..(position + read));
            return read;
        }

        static int ReadMultiLineComment(SyntaxTree syntaxTree, int position, out Trivia trivia)
        {
            var done = false;
            // Skip '/*'.
            var read = 2;
            while (!done)
            {
                switch (syntaxTree.Source[(position + read)..])
                {
                    case []:
                    case ['\0', ..]:
                        syntaxTree.Diagnostics.ReportUnterminatedComment(new SourceLocation(syntaxTree.Source, (position + read)..(position + read + 2)));
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

            trivia = new Trivia(syntaxTree, TokenKind.MultiLineComment, position..(position + read));
            return read;
        }

        static int ReadSingleLineComment(SyntaxTree syntaxTree, int position, out Trivia trivia)
        {
            // Skip '//'.
            var read = 2;
            while (syntaxTree.Source[position + read] is not '\r' and not '\n' and not '\0')
                read++;

            trivia = new Trivia(syntaxTree, TokenKind.SingleLineComment, position..(position + read));
            return read;
        }

        static int ReadNumberToken(SyntaxTree syntaxTree, int position, out TokenKind kind, out Range range, out object? value)
        {
            var read = 0;
            var isInteger = syntaxTree.Source[position + read] != '.';
            do
            {
                read++;
            }
            while (Char.IsDigit(syntaxTree.Source[position + read]));

            // Scan the decimal part of a floating point?
            if (isInteger && syntaxTree.Source[(position + read)..] is ['.', '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9', ..])
            {
                isInteger = false;
                // We stop when there is not dot after a number. This way we grab all dots in a number (which we report as incorrect).
                while (true)
                {
                    do
                    {
                        read++;
                    }
                    while (Char.IsDigit(syntaxTree.Source[position + read]));

                    // Keep consuming if the next input is a single '.'.
                    if (syntaxTree.Source[(position + read)..] is not ['.', not '.', ..])
                        break;
                }
            }

            var localCopyOfRange = range = position..(position + read);
            var text = syntaxTree.Source[range];
            (kind, value) = isInteger
                ? (TokenKind.I32, EnsureCorrectType<int>(text, PredefinedTypes.I32))
                : (TokenKind.F32, EnsureCorrectType<float>(text, PredefinedTypes.F32));

            return read;

            object EnsureCorrectType<T>(ReadOnlySpan<char> text, TypeSymbol type) where T : unmanaged, ISpanParsable<T>
            {
                if (!T.TryParse(text, provider: null, out T value))
                    syntaxTree.Diagnostics.ReportInvalidNumber(new SourceLocation(syntaxTree.Source, localCopyOfRange), text.ToString(), type);
                return value;
            }
        }

        static int ReadStringToken(SyntaxTree syntaxTree, int position, out TokenKind kind, out Range range, out object? value)
        {
            var builder = new StringBuilder();
            var done = false;
            var read = 1;
            while (!done)
            {
                var span = syntaxTree.Source[read..];
                switch (span)
                {
                    case ['\0', ..]:
                    case ['\r', ..]:
                    case ['\n', ..]:
                    case []:
                        syntaxTree.Diagnostics.ReportUnterminatedString(new SourceLocation(syntaxTree.Source, position..(position + 1)));
                        done = true;
                        break;
                    case ['\\', '"', ..]:
                        read++;
                        builder.Append(syntaxTree.Source[read]);
                        read++;
                        break;
                    case ['"', ..]:
                        read++;
                        done = true;
                        break;
                    default:
                        builder.Append(syntaxTree.Source[read]);
                        read++;
                        break;
                }
            }

            kind = TokenKind.String;
            range = position..(position + 1);
            value = builder.ToString();
            return read;
        }

        static int ReadIdentifier(SyntaxTree syntaxTree, int position, out TokenKind kind, out Range range, out object? value)
        {
            var read = 0;
            do
            {
                read++;
            }
            while (Char.IsLetterOrDigit(syntaxTree.Source[read]));

            range = position..(position + read);
            var text = syntaxTree.Source[range];
            kind = text.GetKeywordKind();
            value = null;
            return read;
        }
    }
}
