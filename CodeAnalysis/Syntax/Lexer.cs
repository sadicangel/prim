namespace CodeAnalysis.Syntax;

public sealed class Lexer
{
    private readonly string _text;
    private readonly DiagnosticBag _diagnostics = new();

    public Lexer(string text)
    {
        _text = text;
    }

    public IEnumerable<Diagnostic> Diagnostics { get => _diagnostics; }

    public int Position { get; private set; }

    private char Current { get => Peek(0); }

    private char Lookahead { get => Peek(1); }

    public bool IsEOF { get => Position >= _text.Length; }

    private char Peek(int offset)
    {
        var index = Position + offset;
        return index < _text.Length ? _text[index] : '\0';
    }

    private int Next(int stride = 1)
    {
        var position = Position;
        Position += stride;
        return position;
    }

    public Token NextToken()
    {
        if (IsEOF)
        {
            return new Token(TokenKind.EOF, Position, "\0");
        }

        var start = Position;

        if (char.IsDigit(Current))
        {
            do
            {
                Next();
            }
            while (char.IsDigit(Current));

            var length = Position - start;
            var text = _text.Substring(start, length);
            if (!long.TryParse(text, out var value))
                _diagnostics.ReportInvalidNumber(new TextSpan(start, length), text, typeof(long));
            return new Token(TokenKind.Int64, Position, text, value);
        }

        if (char.IsWhiteSpace(Current))
        {
            do
            {
                Next();
            }
            while (char.IsWhiteSpace(Current));

            var length = Position - start;
            var text = _text.Substring(start, length);

            return new Token(TokenKind.WhiteSpace, Position, text);
        }

        if (char.IsLetter(Current))
        {
            do
            {
                Next();
            }
            while (char.IsLetter(Current));

            var length = Position - start;
            var text = _text.Substring(start, length);
            var kind = text.GetKeywordKind();

            return new Token(kind, Position, text);
        }

        switch (Current)
        {
            case '+':
                return new Token(TokenKind.Plus, Next(), "+");
            case '-':
                return new Token(TokenKind.Minus, Next(), "-");
            case '*':
                return new Token(TokenKind.Star, Next(), "*");
            case '/':
                return new Token(TokenKind.Slash, Next(), "/");
            case '(':
                return new Token(TokenKind.OpenParenthesis, Next(), "(");
            case ')':
                return new Token(TokenKind.CloseParenthesis, Next(), ")");
            case '!' when Lookahead is '=':
                return new Token(TokenKind.BangEquals, Next(2), "!=");
            case '!':
                return new Token(TokenKind.Bang, Next(), "!");
            case '=' when Lookahead is '=':
                return new Token(TokenKind.EqualsEquals, Next(2), "==");
            case '=':
                return new Token(TokenKind.Equals, Next(), "=");
            case '&' when Lookahead is '&':
                return new Token(TokenKind.AmpersandAmpersand, Next(2), "&&");
            case '|' when Lookahead is '|':
                return new Token(TokenKind.PipePipe, Next(2), "||");
        }

        _diagnostics.ReportInvalidCharacter(Position, Current);
        return new Token(TokenKind.Invalid, Next(), _text.Substring(Position - 1, 1));
    }
}
