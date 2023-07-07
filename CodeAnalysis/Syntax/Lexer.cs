using CodeAnalysis.Symbols;
using CodeAnalysis.Text;
using System.Text;

namespace CodeAnalysis.Syntax;

internal readonly record struct LexResult(IReadOnlyList<Token> Tokens, IEnumerable<Diagnostic> Diagnostics);

internal sealed class Lexer
{
    private readonly SyntaxTree _syntaxTree;
    private readonly DiagnosticBag _diagnostics = new();

    private int _position;
    private int _start;
    private TokenKind _kind;
    private object? _value;

    private Lexer(SyntaxTree syntaxTree)
    {
        _syntaxTree = syntaxTree;
    }

    public SourceText Text { get => _syntaxTree.Text; }

    public IEnumerable<Diagnostic> Diagnostics { get => _diagnostics; }

    private char Current { get => Peek(0); }

    public bool IsEOF { get => _position >= Text.Length; }

    public static LexResult Lex(SyntaxTree syntaxTree, Func<Token, bool>? predicate = null)
    {
        var lexer = new Lexer(syntaxTree);
        var tokens = new List<Token>();
        Token token;
        do
        {
            token = lexer.NextToken();
            if (predicate is null || predicate.Invoke(token))
                tokens.Add(token);
        }
        while (token.TokenKind != TokenKind.EOF);

        return new(tokens, lexer.Diagnostics);
    }

    private char Peek(int offset)
    {
        var index = _position + offset;
        return index < Text.Length ? Text[index] : '\0';
    }

    private Token NextToken()
    {
        _start = _position;
        _kind = TokenKind.Invalid;
        _value = null;

        ReadOnlySpan<char> span = Text[_start..];

        switch (span)
        {
            // Punctuation
            case ['=', '>', ..]:
                _kind = TokenKind.Arrow;
                _position += 2;
                break;

            case ['{', ..]:
                _kind = TokenKind.OpenBrace;
                _position++;
                break;

            case ['(', ..]:
                _kind = TokenKind.OpenParenthesis;
                _position++;
                break;

            case ['}', ..]:
                _kind = TokenKind.CloseBrace;
                _position++;
                break;

            case [')', ..]:
                _kind = TokenKind.CloseParenthesis;
                _position++;
                break;

            case [':', ..]:
                _kind = TokenKind.Colon;
                _position++;
                break;

            case [';', ..]:
                _kind = TokenKind.Semicolon;
                _position++;
                break;

            case [',', ..]:
                _kind = TokenKind.Comma;
                _position++;
                break;

            // Operators
            case ['&', '&', ..]:
                _kind = TokenKind.AmpersandAmpersand;
                _position += 2;
                break;

            case ['&', ..]:
                _kind = TokenKind.Ampersand;
                _position++;
                break;

            case ['!', '=', ..]:
                _kind = TokenKind.BangEqual;
                _position += 2;
                break;

            case ['!', ..]:
                _kind = TokenKind.Bang;
                _position++;
                break;

            case ['=', '=', ..]:
                _kind = TokenKind.EqualEqual;
                _position += 2;
                break;

            case ['=', ..]:
                _kind = TokenKind.Equal;
                _position++;
                break;

            case ['>', '=', ..]:
                _kind = TokenKind.GreaterEqual;
                _position += 2;
                break;

            case ['>', ..]:
                _kind = TokenKind.Greater;
                _position++;
                break;

            case ['^', ..]:
                _kind = TokenKind.Hat;
                _position++;
                break;

            case ['<', '=', ..]:
                _kind = TokenKind.LessEqual;
                _position += 2;
                break;

            case ['<', ..]:
                _kind = TokenKind.Less;
                _position++;
                break;

            case ['-', ..]:
                _kind = TokenKind.Minus;
                _position++;
                break;

            case ['%', ..]:
                _kind = TokenKind.Percent;
                _position++;
                break;

            case ['|', '|', ..]:
                _kind = TokenKind.PipePipe;
                _position += 2;
                break;

            case ['|', ..]:
                _kind = TokenKind.Pipe;
                _position++;
                break;

            case ['+', ..]:
                _kind = TokenKind.Plus;
                _position++;
                break;

            case ['.', '.', ..]:
                _kind = TokenKind.Range;
                _position += 2;
                break;

            case ['/', ..]:
                _kind = TokenKind.Slash;
                _position++;
                break;

            case ['*', ..]:
                _kind = TokenKind.Star;
                _position++;
                break;

            case ['~', ..]:
                _kind = TokenKind.Tilde;
                _position++;
                break;

            case ['"', ..]:
                ReadStringToken();
                break;

            case ['0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9', ..]:
            case ['.', '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9', ..]:
                ReadNumberToken();
                break;

            case ['A' or 'B' or 'C' or 'D' or 'E' or 'F' or 'G' or 'H' or 'I' or 'J' or 'K' or 'L' or 'M' or 'N' or 'O' or 'P' or 'Q' or 'R' or 'S' or 'T' or 'U' or 'V' or 'W' or 'X' or 'Y' or 'Z' or
                 'a' or 'b' or 'c' or 'd' or 'e' or 'f' or 'g' or 'h' or 'i' or 'j' or 'k' or 'l' or 'm' or 'n' or 'o' or 'p' or 'q' or 'r' or 's' or 't' or 'u' or 'v' or 'w' or 'x' or 'y' or 'z', ..]:
                ReadIdentifier();
                break;

            case [' ' or '\t' or '\n' or '\r', ..]:
                ReadWhiteSpace();
                break;

            case [var whitespace, ..] when Char.IsWhiteSpace(whitespace):
                ReadWhiteSpace();
                break;

            // Control
            case []:
                _kind = TokenKind.EOF;
                break;

            default:
                _diagnostics.ReportInvalidCharacter(new TextLocation(Text, new TextSpan(_position, 1)), Current);
                _kind = TokenKind.Invalid;
                _position++;
                break;
        }

        var text = SyntaxFacts.GetText(_kind) ?? span[..(_position - _start)];

        return new Token(_syntaxTree, _kind, _start, text.ToString(), _value);
    }

    private void ReadIdentifier()
    {
        do
        {
            _position++;
        }
        while (Char.IsLetterOrDigit(Current));

        var text = Text[_start.._position];
        _kind = text.GetKeywordKind();
    }

    private void ReadWhiteSpace()
    {
        do
        {
            _position++;
        }
        while (Char.IsWhiteSpace(Current));

        _kind = TokenKind.WhiteSpace;
    }

    private void ReadNumberToken()
    {
        var isInteger = Current != '.';
        do
        {
            _position++;
        }
        while (Char.IsDigit(Current));

        // Scan the decimal part of a floating point?
        if (isInteger && Text[_position..] is ['.', '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9', ..])
        {
            isInteger = false;
            // We stop when there is not dot after a number. This way we grab all dots in a number (which we report as incorrect).
            while (true)
            {
                do
                {
                    _position++;
                }
                while (Char.IsDigit(Current));

                // Keep consuming if the next input is a single '.'.
                if (Text[_position..] is not ['.', not '.', ..])
                    break;
            }
        }



        var text = Text[_start.._position];

        (_kind, _value) = isInteger
            ? (TokenKind.I32, EnsureCorrectType<int>(text, BuiltinTypes.I32))
            : (TokenKind.F32, EnsureCorrectType<float>(text, BuiltinTypes.F32));

        object EnsureCorrectType<T>(ReadOnlySpan<char> text, TypeSymbol type) where T : unmanaged, ISpanParsable<T>
        {
            if (!T.TryParse(text, provider: null, out T value))
                _diagnostics.ReportInvalidNumber(new TextLocation(Text, TextSpan.FromBounds(_start, _position)), text.ToString(), type);
            return value;
        }
    }

    private void ReadStringToken()
    {
        var builder = new StringBuilder();
        var done = false;
        _position++;
        while (!done)
        {
            var span = Text[_position..];
            switch (span)
            {
                case ['\0', ..]:
                case ['\r', ..]:
                case ['\n', ..]:
                case []:
                    _diagnostics.ReportUnterminatedString(new TextLocation(Text, new TextSpan(_start, 1)));
                    done = true;
                    break;
                case ['\\', '"', ..]:
                    _position++;
                    builder.Append(Current);
                    _position++;
                    break;
                case ['"', ..]:
                    _position++;
                    done = true;
                    break;
                default:
                    builder.Append(Current);
                    _position++;
                    break;
            }
        }

        _kind = TokenKind.String;
        _value = builder.ToString();
    }
}
