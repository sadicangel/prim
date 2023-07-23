using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Statements;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Syntax;

internal readonly record struct ParseResult(CompilationUnit CompilationUnit, IEnumerable<Diagnostic> Diagnostics);

internal sealed class Parser
{
    private readonly SyntaxTree _syntaxTree;
    private readonly IReadOnlyList<Token> _tokens;
    private readonly DiagnosticBag _diagnostics = new();
    private int _successiveMatchTokenErrors = 0;
    private const int MaxSuccessiveMatchTokenErrors = 1;

    private Parser(SyntaxTree syntaxTree)
    {
        var badTokens = new List<Token>();
        var (tokens, diagnostics) = Lexer.Lex(syntaxTree, (ref Token token) =>
        {
            if (token.TokenKind is TokenKind.Invalid)
            {
                badTokens.Add(token);
                return false;
            }

            if (badTokens.Count > 0)
            {
                var leadingTrivia = new List<Trivia>();
                foreach (var badToken in badTokens)
                {
                    foreach (var trivia in badToken.LeadingTrivia)
                        leadingTrivia.Add(trivia);

                    leadingTrivia.Add(new Trivia(syntaxTree, TokenKind.InvalidText, badToken.Position, badToken.Text));

                    foreach (var trivia in badToken.TrailingTrivia)
                        leadingTrivia.Add(trivia);
                }

                badTokens.Clear();

                leadingTrivia.AddRange(token.LeadingTrivia);

                token = token with { LeadingTrivia = leadingTrivia };
            }

            return true;
        });

        _syntaxTree = syntaxTree;
        _tokens = tokens;
        _diagnostics.AddRange(diagnostics);
    }

    public IEnumerable<Diagnostic> Diagnostics { get => _diagnostics; }

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

    private Token MatchToken(TokenKind kind)
    {
        if (!TryMatchToken(kind, out var token))
        {

            if (_successiveMatchTokenErrors++ < MaxSuccessiveMatchTokenErrors)
                _diagnostics.ReportUnexpectedToken(kind, Current);
            return new Token(_syntaxTree, kind, Current.Position, "", Array.Empty<Trivia>(), Array.Empty<Trivia>());
        }
        _successiveMatchTokenErrors = 0;
        return token;
    }

    private bool TryMatchToken(TokenKind kind, [MaybeNullWhen(false)] out Token token)
    {
        if (Current.TokenKind == kind)
        {
            token = NextToken();
            return true;
        }
        token = null;
        return false;
    }

    private bool TryMatchTokenSequence(ReadOnlySpan<TokenKind> kinds)
    {
        for (var i = 0; i < kinds.Length; ++i)
        {
            if (Peek(i).TokenKind != kinds[i])
                return false;
        }
        return true;
    }

    public static ParseResult Parse(SyntaxTree syntaxTree)
    {
        var parser = new Parser(syntaxTree);
        var compilationUnit = parser.ParseCompilationUnit();
        return new(compilationUnit, parser.Diagnostics);
    }

    private CompilationUnit ParseCompilationUnit()
    {
        var nodes = ParseGlobalNodes();
        var eofToken = MatchToken(TokenKind.EOF);
        return new CompilationUnit(_syntaxTree, nodes, eofToken);
    }

    private IReadOnlyList<GlobalSyntaxNode> ParseGlobalNodes()
    {
        var nodes = new List<GlobalSyntaxNode>();
        while (Current.TokenKind is not TokenKind.EOF)
        {
            var startToken = Current;

            var node = ParseGlobalNode();
            nodes.Add(node);

            // No tokens consumed. Skip the current token to avoid infinite loop.
            // No need to report any extra error as parse methods already failed.
            if (Current == startToken)
                NextToken();
        }
        return nodes;
    }

    private GlobalSyntaxNode ParseGlobalNode()
    {
        if (Current.TokenKind is TokenKind.Let or TokenKind.Var)
            return ParseGlobalDeclaration();
        return ParseGlobalStatement();
    }

    private GlobalDeclaration ParseGlobalDeclaration()
    {
        var declaration = ParseDeclaration();
        return new GlobalDeclaration(_syntaxTree, declaration);
    }

    private GlobalStatement ParseGlobalStatement()
    {
        var statement = ParseStatement();
        return new GlobalStatement(_syntaxTree, statement);
    }

    private Statement ParseStatement()
    {
        return Current.TokenKind switch
        {
            TokenKind.OpenBrace => ParseBlockStatement(),
            TokenKind.Let or
            TokenKind.Var => ParseDeclaration(),
            TokenKind.If => ParseIfStatement(),
            TokenKind.For => ParseForStatement(),
            TokenKind.While => ParseWhileStatement(),
            TokenKind.Break => ParseBreakStatement(),
            TokenKind.Continue => ParseContinueStatement(),
            TokenKind.Return => ParseReturnStatement(),
            _ => ParseExpressionStatement(),
        };
    }

    private Statement ParseBlockStatement()
    {
        var statements = new List<Statement>();
        var openBraceToken = MatchToken(TokenKind.OpenBrace);
        while (Current.TokenKind is not TokenKind.EOF and not TokenKind.CloseBrace)
        {
            var startToken = Current;

            var statement = ParseStatement();
            statements.Add(statement);

            // No tokens consumed. Skip the current token to avoid infinite loop.
            // No need to report any extra error as parse methods already failed.
            if (Current == startToken)
                NextToken();
        }
        var closeBraceToken = MatchToken(TokenKind.CloseBrace);
        return new BlockStatement(_syntaxTree, openBraceToken, statements, closeBraceToken);
    }

    private SeparatedNodeList<T> ParseSeparatedList<T>(Func<T> parseNode) where T : SyntaxNode
    {
        var nodes = new List<SyntaxNode>();

        var parseNext = true;
        while (parseNext && Current.TokenKind is not TokenKind.CloseParenthesis and not TokenKind.EOF)
        {
            var node = parseNode();
            nodes.Add(node);

            if (Current.TokenKind is TokenKind.Comma)
            {
                var commaToken = MatchToken(TokenKind.Comma);
                nodes.Add(commaToken);
            }
            else
            {
                parseNext = false;
            }
        }

        return new SeparatedNodeList<T>(nodes.ToArray());
    }

    private Declaration ParseDeclaration()
    {
        ReadOnlySpan<TokenKind> funcSequence = stackalloc TokenKind[]
        {
            TokenKind.Let,
            TokenKind.Identifier,
            TokenKind.Colon,
            TokenKind.OpenParenthesis
        };

        if (TryMatchTokenSequence(funcSequence))
            return ParseFunctionDeclaration();

        return ParseVariableDeclaration();
    }

    private Declaration ParseVariableDeclaration()
    {
        var modifier = MatchToken(Current.TokenKind is TokenKind.Var ? TokenKind.Var : TokenKind.Let);
        var identifier = MatchToken(TokenKind.Identifier);
        var type = default(Token);
        if (TryMatchToken(TokenKind.Colon, out var colon))
        {
            type = MatchToken(TokenKind.Identifier);
        }
        var equal = MatchToken(TokenKind.Equal);
        var expression = ParseExpression();
        var semicolon = MatchToken(TokenKind.Semicolon);
        return new VariableDeclaration(_syntaxTree, modifier, identifier, colon, type, equal, expression, semicolon);
    }

    private Declaration ParseFunctionDeclaration()
    {
        var modifier = MatchToken(TokenKind.Let);
        var identifier = MatchToken(TokenKind.Identifier);
        var colon = MatchToken(TokenKind.Colon);
        var openParenthesis = MatchToken(TokenKind.OpenParenthesis);
        var parameters = ParseSeparatedList(ParseParameter);
        var closeParenthesis = MatchToken(TokenKind.CloseParenthesis);
        var arrow = MatchToken(TokenKind.Arrow);
        var type = MatchToken(TokenKind.Identifier);
        var equal = MatchToken(TokenKind.Equal);
        var body = (BlockStatement)ParseBlockStatement();
        return new FunctionDeclaration(_syntaxTree, modifier, identifier, colon, openParenthesis, parameters, closeParenthesis, arrow, type, equal, body);

        Parameter ParseParameter()
        {
            var identifier = MatchToken(TokenKind.Identifier);
            var colon = MatchToken(TokenKind.Colon);
            var type = MatchToken(TokenKind.Identifier);
            return new Parameter(_syntaxTree, identifier, colon, type);
        }
    }

    private Statement ParseIfStatement()
    {
        var ifToken = MatchToken(TokenKind.If);
        var openParenthesis = MatchToken(TokenKind.OpenParenthesis);
        var condition = ParseExpression();
        var closeParenthesis = MatchToken(TokenKind.CloseParenthesis);
        var then = ParseStatement();
        if (!TryMatchToken(TokenKind.Else, out var elseToken))
            return new IfStatement(_syntaxTree, ifToken, openParenthesis, condition, closeParenthesis, then);
        var @else = ParseStatement();
        return new IfStatement(_syntaxTree, ifToken, openParenthesis, condition, closeParenthesis, then, elseToken, @else);
    }

    private Statement ParseWhileStatement()
    {
        var whileToken = MatchToken(TokenKind.While);
        var openParenthesis = MatchToken(TokenKind.OpenParenthesis);
        var condition = ParseAssignmentExpression();
        var closeParenthesis = MatchToken(TokenKind.CloseParenthesis);
        var body = ParseStatement();
        return new WhileStatement(_syntaxTree, whileToken, openParenthesis, condition, closeParenthesis, body);
    }

    private Statement ParseForStatement()
    {
        var forToken = MatchToken(TokenKind.For);
        var openParenthesis = MatchToken(TokenKind.OpenParenthesis);
        var let = MatchToken(TokenKind.Let);
        var identifier = MatchToken(TokenKind.Identifier);
        var @in = MatchToken(TokenKind.In);
        var lowerBound = ParseExpression();
        var rangeToken = MatchToken(TokenKind.Range);
        var upperBound = ParseExpression();
        var closeParenthesis = MatchToken(TokenKind.CloseParenthesis);
        var body = ParseStatement();
        return new ForStatement(_syntaxTree, forToken, openParenthesis, let, identifier, @in, lowerBound, rangeToken, upperBound, closeParenthesis, body);
    }

    private Statement ParseBreakStatement()
    {
        var @break = MatchToken(TokenKind.Break);
        var semicolon = MatchToken(TokenKind.Semicolon);
        return new BreakStatement(_syntaxTree, @break, semicolon);
    }

    private Statement ParseContinueStatement()
    {
        var @continue = MatchToken(TokenKind.Continue);
        var semicolon = MatchToken(TokenKind.Semicolon);
        return new ContinueStatement(_syntaxTree, @continue, semicolon);
    }

    private Statement ParseReturnStatement()
    {
        var @return = MatchToken(TokenKind.Return);
        Expression? expression = null;
        if (!TryMatchToken(TokenKind.Semicolon, out var semicolon))
        {
            expression = ParseExpression();
            semicolon = MatchToken(TokenKind.Semicolon);
        }
        return new ReturnStatement(_syntaxTree, @return, expression, semicolon);
    }

    private Statement ParseExpressionStatement()
    {
        var expression = ParseExpression();
        TryMatchToken(TokenKind.Semicolon, out var semicolonToken);
        return new ExpressionStatement(_syntaxTree, expression, semicolonToken);
    }

    private Expression ParseExpression()
    {
        return ParseAssignmentExpression();
    }

    private Expression ParseAssignmentExpression()
    {
        if (Peek(0).TokenKind == TokenKind.Identifier)
        {
            var operatorOrEqual = Peek(1).TokenKind;
            switch ((operatorOrEqual, Peek(2).TokenKind))
            {
                case (TokenKind.Equal, _):
                    {
                        var identifier = MatchToken(TokenKind.Identifier);
                        var equal = MatchToken(TokenKind.Equal);
                        var right = ParseAssignmentExpression();
                        return new AssignmentExpression(_syntaxTree, identifier, equal, right);
                    }
                case (TokenKind.Ampersand, TokenKind.Equal):
                case (TokenKind.Hat, TokenKind.Equal):
                case (TokenKind.Minus, TokenKind.Equal):
                case (TokenKind.Percent, TokenKind.Equal):
                case (TokenKind.Pipe, TokenKind.Equal):
                case (TokenKind.Plus, TokenKind.Equal):
                case (TokenKind.Slash, TokenKind.Equal):
                case (TokenKind.Star, TokenKind.Equal):
                    {
                        var identifier = MatchToken(TokenKind.Identifier);
                        var @operator = MatchToken(operatorOrEqual);
                        var equal = MatchToken(TokenKind.Equal);
                        var right = ParseAssignmentExpression();
                        return new CompoundAssignmentExpression(_syntaxTree, identifier, @operator, equal, right);
                    }
                default:
                    break;
            }
        }


        return ParseBinaryExpression();
    }

    private Expression ParseBinaryExpression(int parentPrecedence = 0)
    {
        Expression left;
        var unaryPrecedence = Current.TokenKind.GetUnaryOperatorPrecedence();
        if (unaryPrecedence != 0 && unaryPrecedence >= parentPrecedence)
        {
            var operationToken = NextToken();
            var operand = ParseBinaryExpression(unaryPrecedence);
            left = new UnaryExpression(_syntaxTree, operationToken, operand);
        }
        else
        {
            left = ParsePrimaryExpression();
        }

        while (true)
        {
            var precedence = Current.TokenKind.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence)
                break;

            var operatorToken = NextToken();
            var right = ParseBinaryExpression(precedence);
            left = new BinaryExpression(_syntaxTree, left, operatorToken, right);
        }
        return left;
    }

    private Expression ParsePrimaryExpression()
    {
        return Current.TokenKind switch
        {
            TokenKind.OpenParenthesis => ParseGroupExpression(),
            TokenKind.False or TokenKind.True => ParseBooleanLiteralExpression(Current.TokenKind),
            TokenKind.I32 or TokenKind.F32 => ParseNumberLiteralExpression(Current.TokenKind),
            TokenKind.String => ParseStringLiteralExpression(),
            _ => ParseNameOrCallExpression(),
        };
    }

    private Expression ParseGroupExpression()
    {
        var left = MatchToken(TokenKind.OpenParenthesis);
        var expression = ParseExpression();
        var right = MatchToken(TokenKind.CloseParenthesis);
        return new GroupExpression(_syntaxTree, left, expression, right);
    }

    private Expression ParseBooleanLiteralExpression(TokenKind booleanTokenKind)
    {
        if (!booleanTokenKind.IsBoolean())
            throw new InvalidOperationException($"Invalid boolean token kind {booleanTokenKind}");
        var literal = MatchToken(booleanTokenKind);
        return new LiteralExpression(_syntaxTree, literal, booleanTokenKind is TokenKind.True);
    }

    private Expression ParseNumberLiteralExpression(TokenKind numberTokenKind)
    {
        if (!numberTokenKind.IsNumber())
            throw new InvalidOperationException($"Invalid number token kind {numberTokenKind}");
        var literal = MatchToken(numberTokenKind);
        return new LiteralExpression(_syntaxTree, literal);
    }

    private Expression ParseStringLiteralExpression()
    {
        var literal = MatchToken(TokenKind.String);
        return new LiteralExpression(_syntaxTree, literal);
    }

    private Expression ParseNameOrCallExpression()
    {
        if (Current.TokenKind is TokenKind.Identifier && Peek(1).TokenKind is TokenKind.OpenParenthesis)
            return ParseCallExpression();
        else
            return ParseNameExpression();
    }

    private Expression ParseCallExpression()
    {
        var identifierToken = MatchToken(TokenKind.Identifier);
        var openParenthesisToken = MatchToken(TokenKind.OpenParenthesis);
        var arguments = ParseSeparatedList(ParseExpression);
        var closeParenthesisToken = MatchToken(TokenKind.CloseParenthesis);
        return new CallExpression(_syntaxTree, identifierToken, openParenthesisToken, arguments, closeParenthesisToken);
    }

    private Expression ParseNameExpression()
    {
        var identifierToken = MatchToken(TokenKind.Identifier);
        return new NameExpression(_syntaxTree, identifierToken);
    }
}