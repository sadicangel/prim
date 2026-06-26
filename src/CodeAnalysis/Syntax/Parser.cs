using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

internal sealed class Parser
{
    private readonly Scanner _scanner;
    private readonly SyntaxTokenStream _stream;
    private readonly DiagnosticBag _diagnostics = [];

    public Parser(SourceText sourceText)
    {
        _scanner = new Scanner(sourceText);
        _stream = new SyntaxTokenStream([.. _scanner.ScanAll()]);
    }

    public IEnumerable<Diagnostic> Diagnostics => _scanner.Diagnostics.Concat(_diagnostics);

    public CompilationUnitSyntax Parse()
    {
        return ParseCompilationUnit();
    }

    private CompilationUnitSyntax ParseCompilationUnit()
    {
        var declarations = ImmutableArray.CreateBuilder<GlobalDeclarationSyntax>();

        while (!_stream.IsAtEnd)
            declarations.Add(ParseGlobalDeclaration());

        var eofToken = Match(SyntaxKind.EofToken);
        return new CompilationUnitSyntax(new SyntaxList<GlobalDeclarationSyntax>(declarations.ToImmutable()), eofToken);
    }

    private SyntaxToken MatchIdentifier(bool allowPrimitiveTypes) => !allowPrimitiveTypes
        ? Match(SyntaxKind.IdentifierToken)
        : MatchAny(
            SyntaxKind.IdentifierToken,
            SyntaxKind.AnyKeyword,
            SyntaxKind.ErrKeyword,
            SyntaxKind.UnknownKeyword,
            SyntaxKind.NeverKeyword,
            SyntaxKind.UnitKeyword,
            SyntaxKind.TypeKeyword,
            SyntaxKind.StrKeyword,
            SyntaxKind.BoolKeyword,
            SyntaxKind.I8Keyword,
            SyntaxKind.I16Keyword,
            SyntaxKind.I32Keyword,
            SyntaxKind.I64Keyword,
            SyntaxKind.IszKeyword,
            SyntaxKind.U8Keyword,
            SyntaxKind.U16Keyword,
            SyntaxKind.U32Keyword,
            SyntaxKind.U64Keyword,
            SyntaxKind.UszKeyword,
            SyntaxKind.F16Keyword,
            SyntaxKind.F32Keyword,
            SyntaxKind.F64Keyword);

    private SimpleNameSyntax ParseSimpleName(bool allowPrimitiveTypes = false)
    {
        var identifier = MatchIdentifier(allowPrimitiveTypes);
        return new SimpleNameSyntax(identifier);
    }

    private NameSyntax ParseName(bool allowPrimitiveTypes = false)
    {
        var identifier = MatchIdentifier(allowPrimitiveTypes);

        if (_stream.PeekKind() != SyntaxKind.DotToken)
            return new SimpleNameSyntax(identifier);

        var syntaxNodes = ImmutableArray.CreateBuilder<SyntaxNode>();
        syntaxNodes.Add(identifier);

        do
        {
            syntaxNodes.Add(Match(SyntaxKind.DotToken));
            syntaxNodes.Add(MatchIdentifier(allowPrimitiveTypes));
        } while (!_stream.IsAtEnd && _stream.PeekKind() == SyntaxKind.DotToken);

        return new QualifiedNameSyntax(new SeparatedSyntaxList<SyntaxToken>(syntaxNodes.ToImmutable()));
    }

    private TypeSyntax ParseType(params ReadOnlySpan<SyntaxKind> endingKinds)
    {
        var syntaxNodes = default(ImmutableArray<SyntaxNode>.Builder);

        var simpleType = ParsePostfixType(endingKinds);

        while (!_stream.IsAtEnd && _stream.PeekKind() == SyntaxKind.BarToken)
        {
            if (syntaxNodes is null)
            {
                syntaxNodes = ImmutableArray.CreateBuilder<SyntaxNode>();
                syntaxNodes.Add(simpleType);
            }

            syntaxNodes.Add(Match(SyntaxKind.BarToken));
            syntaxNodes.Add(ParsePostfixType(endingKinds));
        }

        return syntaxNodes is null
            ? simpleType
            : new UnionTypeSyntax(new SeparatedSyntaxList<TypeSyntax>(syntaxNodes.ToImmutable()));
    }

    private TypeSyntax ParsePostfixType(params ReadOnlySpan<SyntaxKind> endingKinds)
    {
        var type = ParsePrimaryType(endingKinds);

        while (true)
        {
            switch (_stream.PeekKind())
            {
                case SyntaxKind.BracketOpenToken:
                    {
                        var bracketOpenToken = Match(SyntaxKind.BracketOpenToken);
                        var length = _stream.PeekKind() != SyntaxKind.BracketCloseToken
                            ? ParseExpression()
                            : null;
                        var bracketCloseToken = Match(SyntaxKind.BracketCloseToken);

                        type = new ArrayTypeSyntax(type, bracketOpenToken, length, bracketCloseToken);
                        continue;
                    }

                case SyntaxKind.AsteriskToken:
                    {
                        var asteriskToken = Match(SyntaxKind.AsteriskToken);
                        type = new PointerTypeSyntax(type, asteriskToken);
                        continue;
                    }

                case SyntaxKind.HookToken:
                    {
                        var hookToken = Match(SyntaxKind.HookToken);
                        type = new MaybeTypeSyntax(type, hookToken);
                        continue;
                    }

                default:
                    return type;
            }
        }
    }

    private TypeSyntax ParsePrimaryType(params ReadOnlySpan<SyntaxKind> endingKinds)
    {
        var predefinedTypeKind = SyntaxFacts.GetPredefinedTypeKind(_stream.PeekKind());

        if (predefinedTypeKind is not null)
            return new PredefinedTypeSyntax(predefinedTypeKind.Value, _stream.Next());

        if (_stream.PeekKind() == SyntaxKind.ParenthesisOpenToken)
        {
            var parenthesisOpenToken = Match(SyntaxKind.ParenthesisOpenToken);
            var parameters = ImmutableArray.CreateBuilder<SyntaxNode>();

            while (!_stream.IsAtEnd && _stream.PeekKind() != SyntaxKind.ParenthesisCloseToken)
            {
                parameters.Add(ParseType(SyntaxKind.CommaToken, SyntaxKind.ParenthesisCloseToken));

                if (_stream.PeekKind() != SyntaxKind.CommaToken)
                    break;

                parameters.Add(Match(SyntaxKind.CommaToken));
            }

            var parenthesisCloseToken = Match(SyntaxKind.ParenthesisCloseToken);
            var arrowToken = Match(SyntaxKind.MinusGreaterThanToken);
            var returnType = ParseType(endingKinds);

            return new LambdaTypeSyntax(
                parenthesisOpenToken,
                new SeparatedSyntaxList<TypeSyntax>(parameters.ToImmutable()),
                parenthesisCloseToken,
                arrowToken,
                returnType);
        }

        if (_stream.PeekKind() == SyntaxKind.IdentifierToken)
        {
            var name = ParseName();
            return new NamedTypeSyntax(name);
        }

        return CreateErrorType(endingKinds);
    }

    private ErrorTypeSyntax CreateErrorType(params ReadOnlySpan<SyntaxKind> endingKinds)
    {
        var actual = _stream.Peek();

        var isExpectedTerminator =
            actual.Kind == SyntaxKind.EofToken ||
            actual.Kind == SyntaxKind.BarToken ||
            endingKinds.Contains(actual.Kind);

        _diagnostics.ReportUnexpectedToken(SyntaxKind.IdentifierToken, actual);

        if (!isExpectedTerminator)
            _stream.Skip();

        var sourceSpan = new SourceSpan(actual.SourceSpan.SourceText, Range.EmptyAt(actual.SourceSpan.Range.Start.Value));
        var errorToken = SyntaxToken.CreateSynthetic(SyntaxKind.IdentifierToken, sourceSpan);
        return new ErrorTypeSyntax(errorToken);
    }

    private GlobalDeclarationSyntax ParseGlobalDeclaration()
    {
        var name = ParseName();
        var colonToken = Match(SyntaxKind.ColonToken);

        var type = _stream.PeekKind() is SyntaxKind.ColonToken or SyntaxKind.EqualsToken
            ? null
            : ParseType(SyntaxKind.ColonToken, SyntaxKind.EqualsToken);

        var operatorToken = MatchAny(SyntaxKind.ColonToken, SyntaxKind.EqualsToken);
        var initializer = ParseExpression();
        var semicolonToken = Match(SyntaxKind.SemicolonToken);

        return new GlobalDeclarationSyntax(name, colonToken, type, operatorToken, initializer, semicolonToken);
    }

    private LocalDeclarationSyntax ParseLocalDeclaration()
    {
        var name = ParseSimpleName();
        var colonToken = Match(SyntaxKind.ColonToken);

        var type = _stream.PeekKind() is SyntaxKind.ColonToken or SyntaxKind.EqualsToken
            ? null
            : ParseType(SyntaxKind.ColonToken, SyntaxKind.EqualsToken);

        var operatorToken = MatchAny(SyntaxKind.ColonToken, SyntaxKind.EqualsToken);
        var initializer = ParseExpression();
        var semicolonToken = Match(SyntaxKind.SemicolonToken);

        return new LocalDeclarationSyntax(name, colonToken, type, operatorToken, initializer, semicolonToken);
    }

    private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
    {
        var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(_stream.PeekKind());
        ExpressionSyntax expression;
        if (unaryPrecedence > parentPrecedence)
        {
            var operatorToken = _stream.Next();
            var operand = ParseExpression(unaryPrecedence);
            expression = new UnaryExpressionSyntax(operatorToken, operand);
        }
        else
        {
            expression = _stream.PeekKind() switch
            {
                SyntaxKind.ModuleKeyword => ParseModuleExpression(),
                SyntaxKind.TypeKeyword => ParseTypeExpression(),
                SyntaxKind.IfKeyword => ParseIfExpression(),
                SyntaxKind.WhileKeyword => ParseWhileExpression(),
                SyntaxKind.BreakKeyword => ParseBreakExpression(),
                SyntaxKind.ContinueKeyword => ParseContinueExpression(),
                SyntaxKind.ReturnKeyword => ParseReturnExpression(),
                SyntaxKind.BraceOpenToken => ParseBlockExpression(),
                SyntaxKind.ParenthesisOpenToken => !IsLambdaAhead()
                    ? ParseGroupExpression()
                    : ParseLambdaExpression(),
                SyntaxKind.BracketOpenToken => ParseArrayInitializerExpression(),
                SyntaxKind.TrueKeyword
                    or SyntaxKind.FalseKeyword
                    or SyntaxKind.NullKeyword
                    or SyntaxKind.I8LiteralToken
                    or SyntaxKind.U8LiteralToken
                    or SyntaxKind.I16LiteralToken
                    or SyntaxKind.U16LiteralToken
                    or SyntaxKind.I32LiteralToken
                    or SyntaxKind.U32LiteralToken
                    or SyntaxKind.I64LiteralToken
                    or SyntaxKind.U64LiteralToken
                    or SyntaxKind.F16LiteralToken
                    or SyntaxKind.F32LiteralToken
                    or SyntaxKind.F64LiteralToken
                    or SyntaxKind.StrLiteralToken => ParseLiteralExpression(),
                _ => ParseNameExpression()
            };
        }

        while (true)
        {
            switch (_stream.PeekKind())
            {
                case SyntaxKind.EqualsToken:
                    {
                        var equalsToken = _stream.Next();
                        var right = ParseExpression();
                        expression = new AssignmentExpressionSyntax(expression, equalsToken, right);
                        continue;
                    }

                case SyntaxKind.BraceOpenToken:
                    {
                        var braceOpenToken = Match(SyntaxKind.BraceOpenToken);
                        var properties = ImmutableArray.CreateBuilder<SyntaxNode>();

                        while (!_stream.IsAtEnd && _stream.PeekKind() != SyntaxKind.BraceCloseToken)
                        {
                            var propertyName = ParseSimpleName();
                            var equalsToken = Match(SyntaxKind.EqualsToken);
                            var propertyValue = ParseExpression();

                            properties.Add(new PropertyInitializerExpressionSyntax(propertyName, equalsToken, propertyValue));

                            if (_stream.PeekKind() != SyntaxKind.CommaToken)
                                break;

                            properties.Add(Match(SyntaxKind.CommaToken));
                        }

                        var braceCloseToken = Match(SyntaxKind.BraceCloseToken);

                        expression = new ObjectInitializerExpressionSyntax(
                            expression,
                            braceOpenToken,
                            new SeparatedSyntaxList<PropertyInitializerExpressionSyntax>(properties.ToImmutable()),
                            braceCloseToken);

                        continue;
                    }

                case SyntaxKind.ParenthesisOpenToken:
                    {
                        var parenthesisOpenToken = Match(SyntaxKind.ParenthesisOpenToken);
                        var arguments = ImmutableArray.CreateBuilder<SyntaxNode>();

                        while (!_stream.IsAtEnd && _stream.PeekKind() != SyntaxKind.ParenthesisCloseToken)
                        {
                            arguments.Add(ParseExpression());

                            if (_stream.PeekKind() != SyntaxKind.CommaToken)
                                break;

                            arguments.Add(Match(SyntaxKind.CommaToken));
                        }

                        var parenthesisCloseToken = Match(SyntaxKind.ParenthesisCloseToken);

                        expression = new InvocationExpressionSyntax(
                            expression,
                            parenthesisOpenToken,
                            new SeparatedSyntaxList<ExpressionSyntax>(arguments.ToImmutable()),
                            parenthesisCloseToken);

                        continue;
                    }

                case SyntaxKind.BracketOpenToken:
                    {
                        var bracketOpenToken = Match(SyntaxKind.BracketOpenToken);
                        var index = ParseExpression();
                        var bracketCloseToken = Match(SyntaxKind.BracketCloseToken);

                        expression = new ElementAccessExpressionSyntax(
                            expression,
                            bracketOpenToken,
                            index,
                            bracketCloseToken);

                        continue;
                    }

                case SyntaxKind.DotToken:
                    {
                        var dotToken = Match(SyntaxKind.DotToken);
                        var memberName = ParseSimpleName();

                        expression = new MemberAccessExpressionSyntax(expression, dotToken, memberName);
                        continue;
                    }

                case SyntaxKind.AsKeyword:
                    {
                        var asKeyword = Match(SyntaxKind.AsKeyword);
                        var type = ParseType(
                            SyntaxKind.CommaToken,
                            SyntaxKind.ParenthesisCloseToken,
                            SyntaxKind.BracketCloseToken,
                            SyntaxKind.BraceCloseToken,
                            SyntaxKind.SemicolonToken);

                        expression = new ConversionExpressionSyntax(expression, asKeyword, type);
                        continue;
                    }
            }

            var binaryPrecedence = SyntaxFacts.GetBinaryOperatorPrecedence(_stream.PeekKind());

            if (binaryPrecedence > parentPrecedence)
            {
                var operatorToken = _stream.Next();
                var right = ParseExpression(binaryPrecedence);
                expression = new BinaryExpressionSyntax(expression, operatorToken, right);
                continue;
            }

            break;
        }

        return expression;
    }

    private ModuleExpressionSyntax ParseModuleExpression()
    {
        var moduleKeyword = Match(SyntaxKind.ModuleKeyword);
        return new ModuleExpressionSyntax(moduleKeyword);
    }

    private TypeExpressionSyntax ParseTypeExpression()
    {
        var typeKeyword = Match(SyntaxKind.TypeKeyword);
        var braceOpenToken = Match(SyntaxKind.BraceOpenToken);
        var properties = ImmutableArray.CreateBuilder<LocalDeclarationSyntax>();

        while (!_stream.IsAtEnd && _stream.PeekKind() != SyntaxKind.BraceCloseToken)
            properties.Add(ParseLocalDeclaration());

        var braceCloseToken = Match(SyntaxKind.BraceCloseToken);

        return new TypeExpressionSyntax(
            typeKeyword,
            braceOpenToken,
            new SyntaxList<LocalDeclarationSyntax>(properties.ToImmutable()),
            braceCloseToken);
    }

    private IfElseExpressionSyntax ParseIfExpression()
    {
        var ifKeyword = Match(SyntaxKind.IfKeyword);
        var parenthesisOpenToken = Match(SyntaxKind.ParenthesisOpenToken);
        var condition = ParseExpression();
        var parenthesisCloseToken = Match(SyntaxKind.ParenthesisCloseToken);
        var then = ParseExpression();

        if (_stream.PeekKind() == SyntaxKind.SemicolonToken && _stream.Peek(1).Kind == SyntaxKind.ElseKeyword)
            _stream.Skip();

        if (_stream.PeekKind() != SyntaxKind.ElseKeyword)
        {
            return new IfElseExpressionSyntax(
                ifKeyword,
                parenthesisOpenToken,
                condition,
                parenthesisCloseToken,
                then,
                ElseClause: null);
        }

        var elseKeyword = Match(SyntaxKind.ElseKeyword);
        var elseClause = new ElseClauseSyntax(elseKeyword, ParseExpression());

        return new IfElseExpressionSyntax(
            ifKeyword,
            parenthesisOpenToken,
            condition,
            parenthesisCloseToken,
            then,
            elseClause);
    }

    private WhileExpressionSyntax ParseWhileExpression()
    {
        var whileKeyword = Match(SyntaxKind.WhileKeyword);
        var parenthesisOpenToken = Match(SyntaxKind.ParenthesisOpenToken);
        var condition = ParseExpression();
        var parenthesisCloseToken = Match(SyntaxKind.ParenthesisCloseToken);
        var body = ParseExpression();

        return new WhileExpressionSyntax(
            whileKeyword,
            parenthesisOpenToken,
            condition,
            parenthesisCloseToken,
            body);
    }

    private BreakExpressionSyntax ParseBreakExpression()
    {
        var breakKeyword = Match(SyntaxKind.BreakKeyword);
        var expression = IsExpressionTerminator() ? null : ParseExpression();

        return new BreakExpressionSyntax(breakKeyword, expression);
    }

    private ContinueExpressionSyntax ParseContinueExpression()
    {
        var continueKeyword = Match(SyntaxKind.ContinueKeyword);
        var expression = IsExpressionTerminator() ? null : ParseExpression();

        return new ContinueExpressionSyntax(continueKeyword, expression);
    }

    private ReturnExpressionSyntax ParseReturnExpression()
    {
        var returnKeyword = Match(SyntaxKind.ReturnKeyword);
        var expression = IsExpressionTerminator() ? null : ParseExpression();

        return new ReturnExpressionSyntax(returnKeyword, expression);
    }

    private BlockExpressionSyntax ParseBlockExpression()
    {
        var braceOpenToken = Match(SyntaxKind.BraceOpenToken);
        var items = ImmutableArray.CreateBuilder<SyntaxNode>();

        while (!_stream.IsAtEnd && _stream.PeekKind() != SyntaxKind.BraceCloseToken)
        {
            SyntaxNode item;

            if (_stream.PeekKind() == SyntaxKind.SemicolonToken)
            {
                item = new EmptyStatementSyntax(_stream.Next());
            }
            else if (IsDeclarationAhead())
            {
                item = ParseLocalDeclaration();
            }
            else
            {
                var expression = ParseExpression();

                item = _stream.PeekKind() switch
                {
                    SyntaxKind.SemicolonToken => new ExpressionStatementSyntax(expression, _stream.Next()),
                    // TODO: Shouldn't we just accept everything else as expression?
                    SyntaxKind.BraceCloseToken => expression,
                    _ => new ExpressionStatementSyntax(
                        expression,
                        CreateSyntheticAtCurrent(SyntaxKind.SemicolonToken))
                };
            }

            items.Add(item);
        }

        var braceCloseToken = Match(SyntaxKind.BraceCloseToken);

        return new BlockExpressionSyntax(
            braceOpenToken,
            new SyntaxList<SyntaxNode>(items.ToImmutable()),
            braceCloseToken);
    }

    private GroupExpressionSyntax ParseGroupExpression()
    {
        var parenthesisOpenToken = Match(SyntaxKind.ParenthesisOpenToken);
        var expression = ParseExpression();
        var parenthesisCloseToken = Match(SyntaxKind.ParenthesisCloseToken);

        return new GroupExpressionSyntax(parenthesisOpenToken, expression, parenthesisCloseToken);
    }

    private LambdaExpressionSyntax ParseLambdaExpression()
    {
        var parenthesisOpenToken = Match(SyntaxKind.ParenthesisOpenToken);
        var parameters = ImmutableArray.CreateBuilder<SyntaxNode>();

        while (!_stream.IsAtEnd && _stream.PeekKind() != SyntaxKind.ParenthesisCloseToken)
        {
            parameters.Add(ParseSimpleName());

            if (_stream.PeekKind() != SyntaxKind.CommaToken)
                break;

            parameters.Add(Match(SyntaxKind.CommaToken));
        }

        var parenthesisCloseToken = Match(SyntaxKind.ParenthesisCloseToken);
        var equalsGreaterThanToken = Match(SyntaxKind.EqualsGreaterThanToken);
        var body = ParseExpression();

        return new LambdaExpressionSyntax(
            parenthesisOpenToken,
            new SeparatedSyntaxList<SimpleNameSyntax>(parameters.ToImmutable()),
            parenthesisCloseToken,
            equalsGreaterThanToken,
            body);
    }

    private ArrayInitializerExpressionSyntax ParseArrayInitializerExpression()
    {
        var bracketOpenToken = Match(SyntaxKind.BracketOpenToken);
        var elements = ImmutableArray.CreateBuilder<SyntaxNode>();

        while (!_stream.IsAtEnd && _stream.PeekKind() != SyntaxKind.BracketCloseToken)
        {
            elements.Add(ParseExpression());

            if (_stream.PeekKind() != SyntaxKind.CommaToken)
                break;

            elements.Add(Match(SyntaxKind.CommaToken));
        }

        var bracketCloseToken = Match(SyntaxKind.BracketCloseToken);

        return new ArrayInitializerExpressionSyntax(
            bracketOpenToken,
            new SeparatedSyntaxList<ExpressionSyntax>(elements.ToImmutable()),
            bracketCloseToken);
    }

    private LiteralExpressionSyntax ParseLiteralExpression()
    {
        var literal = _stream.Next();
        return new LiteralExpressionSyntax(SyntaxFacts.GetLiteralExpressionKind(literal.Kind), literal, literal.Value ?? Unit.Value);
    }

    private NameExpressionSyntax ParseNameExpression()
    {
        var name = ParseSimpleName(allowPrimitiveTypes: true);
        return new NameExpressionSyntax(name);
    }

    private SyntaxToken Match(SyntaxKind syntaxKind)
    {
        var syntaxToken = _stream.Next();

        if (syntaxToken.Kind == syntaxKind)
            return syntaxToken;

        return CreateSyntheticFrom(syntaxToken, syntaxKind);
    }

    private SyntaxToken MatchAny(SyntaxKind firstSyntaxKind, SyntaxKind secondSyntaxKind, params SyntaxKind[] additionalSyntaxKinds)
    {
        var syntaxToken = _stream.Next();

        if (syntaxToken.Kind == firstSyntaxKind || syntaxToken.Kind == secondSyntaxKind)
            return syntaxToken;

        foreach (var syntaxKind in additionalSyntaxKinds)
        {
            if (syntaxToken.Kind == syntaxKind)
                return syntaxToken;
        }

        return CreateSyntheticFrom(syntaxToken, firstSyntaxKind);
    }

    private SyntaxToken CreateSyntheticFrom(SyntaxToken syntaxToken, SyntaxKind expectedSyntaxKind)
    {
        _diagnostics.ReportUnexpectedToken(expectedSyntaxKind, syntaxToken);
        return SyntaxToken.CreateSynthetic(expectedSyntaxKind, syntaxToken.SourceSpan);
    }

    private SyntaxToken CreateSyntheticAtCurrent(SyntaxKind expectedSyntaxKind)
    {
        var actual = _stream.Peek();

        _diagnostics.ReportUnexpectedToken(expectedSyntaxKind, actual);

        var sourceSpan = new SourceSpan(
            actual.SourceSpan.SourceText,
            Range.EmptyAt(actual.SourceSpan.Range.Start.Value));

        return SyntaxToken.CreateSynthetic(expectedSyntaxKind, sourceSpan);
    }

    private bool IsLambdaAhead()
    {
        if (_stream.PeekKind() != SyntaxKind.ParenthesisOpenToken)
            return false;

        using var checkpoint = _stream.Checkpoint();

        do
        {
            _stream.Skip();
        } while (!_stream.IsAtEnd && _stream.PeekKind() != SyntaxKind.ParenthesisCloseToken);

        _stream.Skip();

        return _stream.PeekKind() == SyntaxKind.EqualsGreaterThanToken;
    }

    private bool IsDeclarationAhead()
    {
        return _stream.PeekKind() == SyntaxKind.IdentifierToken &&
            _stream.Peek(1).Kind == SyntaxKind.ColonToken;
    }

    private bool IsExpressionTerminator()
    {
        return _stream.PeekKind() switch
        {
            SyntaxKind.SemicolonToken or
                SyntaxKind.BraceCloseToken or
                SyntaxKind.ParenthesisCloseToken or
                SyntaxKind.BracketCloseToken or
                SyntaxKind.CommaToken or
                SyntaxKind.ElseKeyword or
                SyntaxKind.EofToken => true,
            _ => false
        };
    }
}
