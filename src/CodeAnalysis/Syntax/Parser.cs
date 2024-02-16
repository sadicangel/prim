using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Text;
using CodeAnalysis.Types;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Syntax;
internal static class Parser
{
    public static CompilationUnit Parse(SyntaxTree syntaxTree)
    {
        IReadOnlyList<Token> tokens = Scanner.Scan(syntaxTree).ToArray();
        if (tokens.Count == 0)
            return new CompilationUnit(syntaxTree, [], new Token(syntaxTree, TokenKind.Eof, Range: default, TokenTrivia.Empty, "\0"));

        var iterator = new TokenIterator(syntaxTree, tokens);
        return new CompilationUnit(
            iterator.SyntaxTree,
            ParseMany(ref iterator, ParseGlobalExpression, static current => current.TokenKind is TokenKind.Eof),
            iterator.Match(TokenKind.Eof));
    }

    public static CompilationUnit ParseScript(SyntaxTree syntaxTree)
    {
        IReadOnlyList<Token> tokens = Scanner.Scan(syntaxTree).ToArray();
        if (tokens.Count == 0)
            return new CompilationUnit(syntaxTree, [], new Token(syntaxTree, TokenKind.Eof, Range: default, TokenTrivia.Empty, "\0"));

        var iterator = new TokenIterator(syntaxTree, tokens);
        return new CompilationUnit(
            iterator.SyntaxTree,
            ParseMany(ref iterator, ParseTerminatedExpression, static current => current.TokenKind is TokenKind.Eof),
            iterator.Match(TokenKind.Eof));
    }

    private static List<T> ParseMany<T>(ref TokenIterator iterator, ParseNode<T> parse, Func<Token, bool> shouldStop)
    {
        var nodes = new List<T>();

        while (!shouldStop(iterator.Current))
        {
            var start = iterator.Current;

            nodes.Add(parse(ref iterator));

            // No tokens consumed. Skip the current token to avoid infinite loop.
            // No need to report any extra error as parse methods already failed.
            if (iterator.Current == start)
                _ = iterator.Next();
        }
        return nodes;
    }

    private static Expression ParseTerminatedExpression(ref TokenIterator iterator)
    {
        return iterator.Current.TokenKind switch
        {
            TokenKind.Semicolon => ParseEmptyExpression(ref iterator),
            TokenKind.BraceOpen => ParseBlockExpression(ref iterator),
            TokenKind.If => ParseIfElseExpression(ref iterator),
            TokenKind.For => ParseForExpression(ref iterator),
            TokenKind.While => ParseWhileExpression(ref iterator),
            TokenKind.Break => ParseBreakExpression(ref iterator),
            TokenKind.Continue => ParseContinueExpression(ref iterator),
            TokenKind.Return => ParseReturnExpression(ref iterator),
            TokenKind.Identifier when iterator.Peek(1).TokenKind == TokenKind.Colon => ParseLocalDeclarationExpression(ref iterator),
            _ => new InlineExpression(
                iterator.SyntaxTree,
                iterator.Current.TokenKind switch
                {
                    _ => ParseBinaryExpression(ref iterator),
                },
                iterator.Match(TokenKind.Semicolon))
        };
    }

    private static EmptyExpression ParseEmptyExpression(ref TokenIterator iterator)
    {
        return new EmptyExpression(iterator.SyntaxTree, iterator.Match(TokenKind.Semicolon));
    }

    private static Expression ParseExpression(ref TokenIterator iterator)
    {
        return iterator.Current.TokenKind switch
        {
            TokenKind.BraceOpen => ParseBlockExpression(ref iterator),
            TokenKind.If => ParseIfElseExpression(ref iterator),
            TokenKind.For => ParseForExpression(ref iterator),
            TokenKind.While => ParseWhileExpression(ref iterator),
            TokenKind.Identifier when iterator.Peek(1).TokenKind == TokenKind.Colon => ParseLocalDeclarationExpression(ref iterator),
            TokenKind.Break => ParseBreakExpression(ref iterator),
            TokenKind.Continue => ParseContinueExpression(ref iterator),
            TokenKind.Return => ParseReturnExpression(ref iterator),
            _ => ParseBinaryExpression(ref iterator),
        };
    }

    private delegate T ParseNode<T>(ref TokenIterator iterator);

    private static SeparatedNodeList<T> ParseSeparatedList<T>(ref TokenIterator iterator, ParseNode<T> parseNode) where T : notnull
    {
        var nodes = new List<object>();

        var parseNext = true;
        while (parseNext && iterator.Current.TokenKind is not TokenKind.ParenthesisClose and not TokenKind.Eof)
        {
            var node = parseNode(ref iterator);
            nodes.Add(node);

            if (iterator.Current.TokenKind is TokenKind.Comma)
                nodes.Add(iterator.Next());
            else
                parseNext = false;
        }

        return new SeparatedNodeList<T>(nodes.ToArray());
    }

    private static PrimType ParseType(ref TokenIterator iterator)
    {
        if (iterator.Current.TokenKind is TokenKind.ParenthesisOpen)
        {
            var parenthesisOpen = iterator.Match(TokenKind.ParenthesisOpen);
            var parameters = ParseSeparatedList(ref iterator, ParseParameter);
            var parenthesisClose = iterator.Match(TokenKind.ParenthesisClose);
            var arrow = iterator.Match(TokenKind.Arrow);
            var returnTypeSyntax = ParseTypeSyntax(ref iterator);

            return new FunctionType(parameters, returnTypeSyntax.Type);
        }
        else
        {
            var typeName = iterator.Current.Text.ToString();
            PrimType type = iterator.Current.TokenKind.IsPredefinedType()
                ? PredefinedTypes.All.Single(t => t.Name == typeName)
                : new UserType(typeName);
            _ = iterator.Next();

            // Check for option or array.
            switch (iterator.Current.TokenKind)
            {
                case TokenKind.Hook:
                    type = new OptionType(type);
                    _ = iterator.Next();
                    break;

                case TokenKind.BracketOpen:
                    _ = iterator.Next();
                    _ = iterator.Match(TokenKind.BracketClose);
                    type = new ArrayType(type);
                    break;
            }

            return type;
        }

        static Parameter ParseParameter(ref TokenIterator iterator)
        {
            var identifier = iterator.Match(TokenKind.Identifier);
            var colon = iterator.Match(TokenKind.Colon);
            var type = ParseType(ref iterator);

            return new Parameter(identifier.Text.ToString(), type);
        }
    }

    private static TypeSyntax ParseTypeSyntax(ref TokenIterator iterator)
    {
        // type_syntax: types 
        // types      : type_name | type_name '|' type_union
        // type_name  : identifier | identifier '?' | identifier '[' ']' | '(' [params]* ')' '->' type_syntax
        // params     : param | param ',' params
        // param      : identifier ':' type_syntax

        var start = iterator.Index;
        var types = ParseSeparatedTypeList(ref iterator, ParseType);
        var type = types switch
        {
        [] => ReportInvalidTypeAndReturnNever(ref iterator),
        [var single] => single,
            _ => new UnionType(types)
        };
        var count = iterator.Index - start;
        return new TypeSyntax(iterator.SyntaxTree, [.. iterator.Tokens.Skip(start).Take(count)], type);

        static PrimType ReportInvalidTypeAndReturnNever(ref TokenIterator iterator)
        {
            iterator.SyntaxTree.Diagnostics.ReportExpectedTypeDefinition(iterator.Current.Location);
            return PredefinedTypes.Never;
        }

        static SeparatedNodeList<PrimType> ParseSeparatedTypeList(ref TokenIterator iterator, ParseNode<PrimType> parseNode)
        {
            var nodes = new List<object>();

            var parseNext = true;
            while (parseNext && iterator.Current.TokenKind is not TokenKind.Equal and not TokenKind.Eof)
            {
                var node = parseNode(ref iterator);
                nodes.Add(node);

                if (iterator.Current.TokenKind is TokenKind.Pipe)
                    nodes.Add(iterator.Next());
                else
                    parseNext = false;
            }

            return new SeparatedNodeList<PrimType>(nodes.ToArray());
        }
    }

    private static BlockExpression ParseBlockExpression(ref TokenIterator iterator)
    {
        // block: '{' [expression ';']* '}'
        var braceOpen = iterator.Match(TokenKind.BraceOpen);
        var expressions = ParseMany(ref iterator, ParseTerminatedExpression, static current => current.TokenKind is TokenKind.Eof or TokenKind.BraceClose);
        var braceClose = iterator.Match(TokenKind.BraceClose);
        return new BlockExpression(iterator.SyntaxTree, braceOpen, expressions, braceClose);
    }

    private static IfElseExpression ParseIfElseExpression(ref TokenIterator iterator)
    {
        // if_else: 'if' '(' expression ')' expression 'else' expression ';'
        var @if = iterator.Match(TokenKind.If);
        var parenthesisOpen = iterator.Match(TokenKind.ParenthesisOpen);
        var condition = ParseExpression(ref iterator);
        var parenthesisClose = iterator.Match(TokenKind.ParenthesisClose);
        var then = ParseExpression(ref iterator);
        var elseToken = iterator.Match(TokenKind.Else);
        var @else = ParseTerminatedExpression(ref iterator);

        return new IfElseExpression(iterator.SyntaxTree, @if, parenthesisOpen, condition, parenthesisClose, then, elseToken, @else);
    }

    private static ForExpression ParseForExpression(ref TokenIterator iterator)
    {
        // for: 'for' '(' identifier ':' expression ')' body ';'
        var @for = iterator.Match(TokenKind.For);
        var parenthesisOpen = iterator.Match(TokenKind.ParenthesisOpen);
        var identifier = iterator.Match(TokenKind.Identifier);
        var colon = iterator.Match(TokenKind.Colon);
        var enumerable = ParseExpression(ref iterator);
        var parenthesisClose = iterator.Match(TokenKind.ParenthesisClose);
        var body = ParseTerminatedExpression(ref iterator);

        return new ForExpression(iterator.SyntaxTree, @for, parenthesisOpen, identifier, colon, enumerable, parenthesisClose, body);
    }

    private static WhileExpression ParseWhileExpression(ref TokenIterator iterator)
    {
        // while: 'while' '(' expression ')' expression ';'
        var @while = iterator.Match(TokenKind.While);
        var parenthesisOpen = iterator.Match(TokenKind.ParenthesisOpen);
        var condition = ParseExpression(ref iterator);
        var parenthesisClose = iterator.Match(TokenKind.ParenthesisClose);
        var body = ParseTerminatedExpression(ref iterator);

        return new WhileExpression(iterator.SyntaxTree, @while, parenthesisOpen, condition, parenthesisClose, body);
    }

    private static BreakExpression ParseBreakExpression(ref TokenIterator iterator)
    {
        // break: 'break' [expression]
        var @break = iterator.Match(TokenKind.Break);
        var result = ParseTerminatedExpression(ref iterator);

        return new BreakExpression(iterator.SyntaxTree, @break, result);
    }

    private static ContinueExpression ParseContinueExpression(ref TokenIterator iterator)
    {
        // break: 'break' [expression]
        var @continue = iterator.Match(TokenKind.Continue);
        var result = ParseTerminatedExpression(ref iterator);

        return new ContinueExpression(iterator.SyntaxTree, @continue, result);
    }

    private static ReturnExpression ParseReturnExpression(ref TokenIterator iterator)
    {
        // break: 'break' [expression]
        var @return = iterator.Match(TokenKind.Return);
        var result = ParseTerminatedExpression(ref iterator);

        return new ReturnExpression(iterator.SyntaxTree, @return, result);
    }

    private static Expression ParseBinaryExpression(ref TokenIterator iterator, int parentPrecedence = 0)
    {
        Expression left;
        var unaryPrecedence = iterator.Current.TokenKind.GetUnaryOperatorPrecedence();
        if (unaryPrecedence != 0 && unaryPrecedence >= parentPrecedence)
        {
            var operationToken = iterator.Next();
            var operand = ParseBinaryExpression(ref iterator, unaryPrecedence);
            left = new UnaryExpression(iterator.SyntaxTree, operationToken, operand);
        }
        else
        {
            left = ParsePrimaryExpression(ref iterator);
        }

        while (true)
        {
            var precedence = iterator.Current.TokenKind.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence)
                break;

            var @operator = iterator.Next();
            var right = @operator.TokenKind is TokenKind.ParenthesisOpen
                 ? new ArgumentListExpression(iterator.SyntaxTree, ParseSeparatedList(ref iterator, ParseExpression))
                 : ParseBinaryExpression(ref iterator, precedence);
            left = @operator.TokenKind switch
            {
                // identifier '.' identifier
                TokenKind.Dot => new BinaryExpression(
                    iterator.SyntaxTree,
                    left,
                    @operator,
                    right),

                // identifier '(' args ')'
                TokenKind.ParenthesisOpen => new BinaryExpression(
                    iterator.SyntaxTree,
                    left,
                    @operator,
                    right,
                    iterator.Match(TokenKind.ParenthesisClose)),

                // identifier '[' expression ']'
                TokenKind.BracketOpen => new BinaryExpression(
                    iterator.SyntaxTree,
                    left,
                    @operator,
                    right,
                    iterator.Match(TokenKind.BracketClose)),

                // Default binary expression.
                _ => new BinaryExpression(iterator.SyntaxTree, left, @operator, right)
            };
        }
        return left;
    }

    private static Expression ParsePrimaryExpression(ref TokenIterator iterator)
    {
        return iterator.Current.TokenKind switch
        {
            // group: '(' expression ')'
            TokenKind.ParenthesisOpen => ParseGroupExpression(ref iterator),

            // any literal
            TokenKind.False or
            TokenKind.True or
            TokenKind.I32 or
            TokenKind.F32 or
            TokenKind.Str or
            TokenKind.Null => ParseLiteralExpression(ref iterator),

            // any identifier
            _ => ParseIdentifierExpression(ref iterator)
        };
    }

    private static GroupExpression ParseGroupExpression(ref TokenIterator iterator)
    {
        return new GroupExpression(
            iterator.SyntaxTree,
            iterator.Match(TokenKind.ParenthesisOpen),
            ParseExpression(ref iterator),
            iterator.Match(TokenKind.ParenthesisClose));
    }

    private static LiteralExpression ParseLiteralExpression(ref TokenIterator iterator)
    {
        var literal = iterator.Next();
        var (type, value) = literal.TokenKind switch
        {
            TokenKind.False => (PredefinedTypes.Bool, false),
            TokenKind.True => (PredefinedTypes.Bool, true),
            TokenKind.I32 => (PredefinedTypes.I32, literal.Value),
            TokenKind.F32 => (PredefinedTypes.F32, literal.Value),
            TokenKind.Str => (PredefinedTypes.Str, literal.Value),
            TokenKind.Null => (PredefinedTypes.Unit, literal.Value),
            _ => throw new UnreachableException($"Unexpected {nameof(TokenKind)} '{literal.TokenKind}' for literal")
        };
        return new LiteralExpression(iterator.SyntaxTree, literal, type, value);
    }

    private static IdentifierExpression ParseIdentifierExpression(ref TokenIterator iterator)
    {
        return iterator.Peek(1).TokenKind switch
        {
            // declaration: identifier ':' type '=' expression
            TokenKind.Colon or TokenKind.Operator => ParseLocalDeclarationExpression(ref iterator),

            // assign: identifier '=' expression
            TokenKind.Equal => ParseAssignmentExpression(ref iterator),

            // compound_assignment: identifier operator expression
            // operator    : '+=' | '-=' | '*=' | '/=' | '%=' | '**=' | '<<=' | '>>=' | '&=' | '|=' | '^=' | '??='
            TokenKind.PlusEqual or
            TokenKind.MinusEqual or
            TokenKind.StarEqual or
            TokenKind.SlashEqual or
            TokenKind.PercentEqual or
            TokenKind.StarStarEqual or
            TokenKind.LessLessEqual or
            TokenKind.GreaterGreaterEqual or
            TokenKind.AmpersandEqual or
            TokenKind.PipeEqual or
            TokenKind.HatEqual or
            TokenKind.HookHookEqual => ParseAssignmentExpression(ref iterator),

            // name: identifier
            _ => ParseNameExpression(ref iterator),
        };
    }

    private static GlobalDeclarationExpression ParseGlobalExpression(ref TokenIterator iterator)
    {
        var declaration = ParseDeclarationExpression(ref iterator);

        return new GlobalDeclarationExpression(iterator.SyntaxTree, declaration);
    }

    private static LocalDeclarationExpression ParseLocalDeclarationExpression(ref TokenIterator iterator)
    {
        var declaration = ParseDeclarationExpression(ref iterator);
        if (declaration.TypeNode.Type is TypeType)
            iterator.SyntaxTree.Diagnostics.ReportInvalidLocationForTypeDefinition(
                new SourceLocation(iterator.SyntaxTree.Source, new Range(declaration.Identifier.Range.Start, declaration.TypeNode.Range.End)));

        return new LocalDeclarationExpression(iterator.SyntaxTree, declaration);
    }

    private static DeclarationExpression ParseDeclarationExpression(ref TokenIterator iterator)
    {
        return iterator.Peek(2).TokenKind switch
        {
            // user_type: identifier ':'         'type' '=' expression
            TokenKind.Type_Type => ParseTypeDeclarationExpression(ref iterator),
            // variable : identifier ':'          type  '=' expression
            // function : identifier ':' function_type  '=' expression
            _ => ParseFuncOrVarDeclarationExpression(ref iterator),
        };
    }

    private static DeclarationExpression ParseFuncOrVarDeclarationExpression(ref TokenIterator iterator)
    {
        var identifier = iterator.Match(TokenKind.Identifier);
        var colon = iterator.Match(TokenKind.Colon);
        var type = ParseTypeSyntax(ref iterator);
        var equal = iterator.Match(TokenKind.Equal);
        var expression = ParseTerminatedExpression(ref iterator);
        return new DeclarationExpression(
            iterator.SyntaxTree,
            identifier,
            colon,
            type,
            equal,
            expression);
    }

    private static DeclarationExpression ParseTypeDeclarationExpression(ref TokenIterator iterator)
    {
        var identifier = iterator.Match(TokenKind.Identifier);
        var colon = iterator.Match(TokenKind.Colon);
        var type = new TypeSyntax(iterator.SyntaxTree, [iterator.Match(TokenKind.Type_Type)], new UserType(identifier.Text.ToString()));
        var equal = iterator.Match(TokenKind.Equal);
        var memberList = ParseMemberListExpression(ref iterator);
        return new DeclarationExpression(
            iterator.SyntaxTree,
            identifier,
            colon,
            type,
            equal,
            memberList);
    }

    private static MemberListExpression ParseMemberListExpression(ref TokenIterator iterator)
    {
        var braceOpen = iterator.Match(TokenKind.BraceOpen);
        var members = ParseMany(ref iterator, ParseMemberExpression, static t => t.TokenKind is TokenKind.Eof or TokenKind.BraceClose);
        var braceClose = iterator.Match(TokenKind.BraceClose);

        return new MemberListExpression(iterator.SyntaxTree, braceOpen, members, braceClose);

        static MemberExpression ParseMemberExpression(ref TokenIterator iterator)
        {
            // variable: identifier ':' mutable? type '=' expression
            // method  : identifier ':' function_type '=' expression

            var declaration = iterator.Peek(2).TokenKind switch
            {
                // user_type: identifier ':'         'type' '=' expression
                TokenKind.Type_Type => ParseTypeDeclarationExpression(ref iterator),
                // property: identifier ':'          type  '=' expression
                // method  : identifier ':' function_type  '=' expression
                _ => ParseFuncOrVarDeclarationExpression(ref iterator),
            };

            var memberType = (MemberType)0;
            switch (declaration.TypeNode.Type)
            {
                case TypeType:
                    iterator.SyntaxTree.Diagnostics.ReportInvalidLocationForTypeDefinition(
                        new SourceLocation(iterator.SyntaxTree.Source, new Range(declaration.Identifier.Range.Start, declaration.TypeNode.Range.End)));
                    break;

                case FunctionType:
                    memberType = MemberType.Method;
                    break;

                default:
                    memberType = MemberType.Property;
                    break;
            }

            return new MemberExpression(iterator.SyntaxTree, memberType, declaration);
        }
    }

    private static AssignmentExpression ParseAssignmentExpression(ref TokenIterator iterator)
    {
        // assignment : identifier '=' expression
        // assign_comp: identifier ['+=' | '-=' | '*=' | '/=' | '%=' | '**=' | '<<=' | '>>=' | '&=' | '|=' | '^=' | '??=']+ expression
        return new AssignmentExpression(
            iterator.SyntaxTree,
            iterator.Match(TokenKind.Identifier),
            iterator.Next(),
            ParseExpression(ref iterator));
    }

    private static NameExpression ParseNameExpression(ref TokenIterator iterator)
    {
        return new NameExpression(iterator.SyntaxTree, iterator.Match(TokenKind.Identifier));
    }
}

internal record struct TokenIterator(SyntaxTree SyntaxTree, IReadOnlyList<Token> Tokens)
{
    private int _successiveMatchTokenErrors = 0;
    private const int MaxSuccessiveMatchTokenErrors = 1;

    public int Index { get; private set; }

    public readonly Token Current { get => Tokens[int.Clamp(Index, 0, Tokens.Count - 1)]; }

    public readonly Token Peek(int offset = 0)
    {
        var index = Index + offset;
        if (index >= Tokens.Count)
            return Tokens[^1];
        return Tokens[index];
    }

    public Token Next()
    {
        var current = Current;
        ++Index;
        return current;
    }

    public Token Match(TokenKind kind)
    {
        if (!TryMatch(kind, out var token))
        {
            if (_successiveMatchTokenErrors++ < MaxSuccessiveMatchTokenErrors)
                SyntaxTree.Diagnostics.ReportUnexpectedToken(kind, Current);
            return new Token(SyntaxTree, kind, Current.Range, TokenTrivia.Empty, "");
        }
        _successiveMatchTokenErrors = 0;
        return token;
    }

    public bool TryMatch(TokenKind kind, [MaybeNullWhen(false)] out Token token) =>
        (token = MatchOrDefault(kind)) is not null;

    public Token? MatchOrDefault(TokenKind kind) => Current.TokenKind == kind ? Next() : null;
}
