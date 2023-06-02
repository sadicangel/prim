namespace CodeAnalysis.Syntax;

internal sealed class Parser
{
    private readonly List<Token> _tokens;
    private readonly DiagnosticBag _diagnostics = new();

    public Parser(string text)
    {
        _tokens = new List<Token>();

        var lexer = new Lexer(text);
        Token token;
        do
        {
            token = lexer.NextToken();
            if (token.Kind is not TokenKind.WhiteSpace and not TokenKind.Invalid)
                _tokens.Add(token);
        }
        while (token.Kind != TokenKind.EOF);
        _diagnostics.AddRange(lexer.Diagnostics);
    }

    private Token Current => Peek(0);

    public int Position { get; private set; }

    private Token Peek(int offset)
    {
        var index = Position + offset;
        if (index >= _tokens.Count)
            return _tokens[^1];
        return _tokens[index];
    }

    private Token NextToken()
    {
        var current = Current;
        Position++;
        return current;
    }

    private Token MatchToken(TokenKind tokenKind)
    {
        if (Current.Kind == tokenKind)
            return NextToken();

        _diagnostics.ReportUnexpectedToken(tokenKind, Current);
        return new Token(tokenKind, Current.Position, "", null);
    }

    public SyntaxTree Parse()
    {
        var expression = ParseExpression();
        var eofToken = MatchToken(TokenKind.EOF);
        return new SyntaxTree(_diagnostics, expression, eofToken);
    }

    private Expression ParseExpression()
    {
        return ParseAssignmentExpression();
    }

    private Expression ParseAssignmentExpression()
    {
        if (Peek(0).Kind == TokenKind.Identifier && Peek(1).Kind == TokenKind.Equals)
        {
            var identifierToken = NextToken();
            var operatorToken = NextToken();
            var right = ParseAssignmentExpression();
            return new AssignmentExpression(identifierToken, operatorToken, right);
        }

        return ParseBinaryExpression();
    }

    private Expression ParseBinaryExpression(int parentPrecedence = 0)
    {
        Expression left;
        var unaryPrecendence = Current.Kind.GetUnaryOperatorPrecendence();
        if (unaryPrecendence != 0 && unaryPrecendence >= parentPrecedence)
        {
            var operationToken = NextToken();
            var operand = ParseBinaryExpression(unaryPrecendence);
            left = new UnaryExpression(operationToken, operand);
        }
        else
        {
            left = ParsePrimaryExpression();
        }

        while (true)
        {
            var precendence = Current.Kind.GetBinaryOperatorPrecendence();
            if (precendence == 0 || precendence <= parentPrecedence)
                break;

            var operatorToken = NextToken();
            var right = ParseBinaryExpression(precendence);
            left = new BinaryExpression(left, operatorToken, right);
        }
        return left;
    }

    private Expression ParsePrimaryExpression()
    {
        switch (Current.Kind)
        {
            case TokenKind.OpenParenthesis:
                return new GroupExpression(NextToken(), ParseExpression(), MatchToken(TokenKind.CloseParenthesis));

            case TokenKind.False or TokenKind.True:
                var booleanToken = NextToken();
                return new LiteralExpression(booleanToken, booleanToken.Kind == TokenKind.True);

            case TokenKind.Identifier:
                return new NameExpression(NextToken());

            default:
                var literal = MatchToken(TokenKind.Int64);
                return new LiteralExpression(literal);
        }
    }
}