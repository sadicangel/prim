﻿namespace CodeAnalysis.Syntax;

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
        return Current.Kind switch
        {
            TokenKind.OpenParenthesis => ParseGroupExpression(),
            TokenKind.False or TokenKind.True => ParseBooleanLiteralExpression(),
            TokenKind.Int64 => ParseNumberLiteralExpression(),
            _ => ParseNameExpression(),
        };
    }

    private Expression ParseGroupExpression()
    {
        var left = MatchToken(TokenKind.OpenParenthesis);
        var expression = ParseExpression();
        var right = MatchToken(TokenKind.CloseParenthesis);
        return new GroupExpression(left, expression, right);
    }

    private Expression ParseBooleanLiteralExpression()
    {
        var isTrue = Current.Kind == TokenKind.True;
        var booleanToken = MatchToken(isTrue ? TokenKind.True : TokenKind.False);
        return new LiteralExpression(booleanToken, isTrue);
    }

    private Expression ParseNumberLiteralExpression()
    {
        var literal = MatchToken(TokenKind.Int64);
        return new LiteralExpression(literal);
    }

    private Expression ParseNameExpression()
    {
        var identifierToken = MatchToken(TokenKind.Identifier);
        return new NameExpression(identifierToken);
    }
}