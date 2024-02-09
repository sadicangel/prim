using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using System.Collections;

namespace CodeAnalysis.Text;

public readonly record struct SymbolKind;

public interface IReadOnlyDiagnosticBag : IReadOnlyList<Diagnostic>
{
    public bool HasErrors { get; }
    public bool HasWarnings { get; }

    public IEnumerable<Diagnostic> GetErrors() => this.Where(d => d.Severity is DiagnosticSeverity.Error);
    public IEnumerable<Diagnostic> GetWarnings() => this.Where(d => d.Severity is DiagnosticSeverity.Warning);
}

public sealed class DiagnosticBag : IReadOnlyDiagnosticBag
{
    private readonly List<Diagnostic> _diagnostics;

    public int Count { get => _diagnostics.Count; }

    public bool HasErrors { get; private set; }

    public bool HasWarnings { get; private set; }

    public Diagnostic this[int index] { get => _diagnostics[index]; }

    public DiagnosticBag() => _diagnostics = new();

    public DiagnosticBag(IEnumerable<Diagnostic> diagnostics)
    {
        _diagnostics = new(diagnostics);
        HasErrors = diagnostics.Any(d => d.Severity is DiagnosticSeverity.Error);
        HasWarnings = diagnostics.Any(d => d.Severity is DiagnosticSeverity.Warning);
    }

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddRange(IEnumerable<Diagnostic> diagnostics)
    {
        _diagnostics.AddRange(diagnostics);
        HasErrors |= diagnostics.Any(d => d.Severity is DiagnosticSeverity.Error);
        HasWarnings |= diagnostics.Any(d => d.Severity is DiagnosticSeverity.Warning);
    }

    public void Report(DiagnosticSeverity severity, SourceLocation location, string message)
    {
        switch (severity)
        {
            case DiagnosticSeverity.Error:
                HasErrors = true;
                break;

            case DiagnosticSeverity.Warning:
                HasWarnings = true;
                break;

            default:
                throw new InvalidOperationException($"Unexpected diagnostic severity {severity}");
        }
        _diagnostics.Add(new Diagnostic(severity, location, message));
    }

    public void ReportError(SourceLocation location, string message) => Report(DiagnosticSeverity.Error, location, message);
    public void ReportWarning(SourceLocation location, string message) => Report(DiagnosticSeverity.Warning, location, message);

    // Scanning Errors.
    internal void ReportInvalidCharacter(SourceLocation location, char character) => ReportError(location, DiagnosticMessage.InvalidCharacter(character));
    internal void ReportInvalidNumber(SourceLocation location, string text, PrimType type) => ReportError(location, DiagnosticMessage.InvalidNumber(text, type));
    internal void ReportUnterminatedComment(SourceLocation location) => ReportError(location, DiagnosticMessage.UnterminatedComment());
    internal void ReportUnterminatedString(SourceLocation location) => ReportError(location, DiagnosticMessage.UnterminatedString());

    // Parsing Errors.
    internal void ReportExpectedTypeDefinition(SourceLocation location) => ReportError(location, DiagnosticMessage.ExpectedTypeDefinition());
    internal void ReportUnexpectedToken(TokenKind expected, Token actual) => ReportError(actual.Location, DiagnosticMessage.UnexpectedToken(expected, actual.TokenKind));

    // Binder Errors.
    internal void ReportAmbiguousBinaryOperator(Token @operator, PrimType leftType, PrimType rightType) => ReportError(@operator.Location, DiagnosticMessage.AmbiguousBinaryOperator(@operator, leftType, rightType));
    internal void ReportInvalidArgumentType(SourceLocation location, Parameter parameter, PrimType actualType) => ReportError(location, DiagnosticMessage.InvalidArgumentType(parameter, actualType));
    internal void ReportInvalidExpressionType(SourceLocation location, PrimType expectedType, PrimType actualType) => ReportError(location, DiagnosticMessage.InvalidExpressionType(expectedType, actualType));
    internal void ReportInvalidImplicitConversion(SourceLocation location, PrimType sourceType, PrimType destinationType) => ReportError(location, DiagnosticMessage.InvalidImplicitConversion(sourceType, destinationType));
    internal void ReportInvalidNumberOfArguments(SourceLocation location, FunctionType functionType, int actualNumberOfArguments) => ReportError(location, DiagnosticMessage.InvalidNumberOfArguments(functionType, actualNumberOfArguments));
    internal void ReportInvalidSymbolType(SourceLocation location, PrimType expectedType, PrimType actualType) => ReportError(location, DiagnosticMessage.InvalidSymbolType(expectedType, actualType));
    internal void ReportInvalidTypeConversion(SourceLocation location, PrimType sourceType, PrimType targetType) => ReportError(location, DiagnosticMessage.InvalidTypeConversion(sourceType, targetType));
    internal void ReportSymbolReassignment(SourceLocation location, Symbol symbol) => ReportError(location, DiagnosticMessage.SymbolReassignment(symbol));
    internal void ReportSymbolRedeclaration(SourceLocation location, Symbol symbol) => ReportError(location, DiagnosticMessage.SymbolRedeclaration(symbol));
    internal void ReportUndefinedBinaryOperator(Token @operator, PrimType leftType, PrimType rightType) => ReportError(@operator.Location, DiagnosticMessage.UndefinedBinaryOperator(@operator, leftType, rightType));
    internal void ReportUndefinedType(SourceLocation location, PrimType type) => ReportError(location, DiagnosticMessage.UndefinedType(type));
    internal void ReportUndefinedSymbol(SourceLocation location, Symbol symbol) => ReportError(location, DiagnosticMessage.ReportUndefinedSymbol(symbol));
    internal void ReportUndefinedUnaryOperator(Token @operator, PrimType operandType) => ReportError(@operator.Location, DiagnosticMessage.UndefinedUnaryOperator(@operator, operandType));




    internal void ReportRedundantConversion(SourceLocation location) => ReportWarning(location, DiagnosticMessage.RedundantConversion());
    internal void ReportInvalidExpressionType(SourceLocation location, PrimType actualType) => ReportError(location, DiagnosticMessage.InvalidExpressionType(actualType));
    internal void ReportInvalidSymbol(Token identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => ReportError(identifierToken.Location, DiagnosticMessage.InvalidSymbol(identifierToken, expectedKind, actualKind));
    internal void ReportInvalidBreakOrContinue(SourceLocation location) => ReportError(location, DiagnosticMessage.InvalidBreakOrContinue());
    internal void ReportInvalidReturn(SourceLocation location) => ReportError(location, DiagnosticMessage.InvalidReturn());
    internal void ReportInvalidReturnExpression(SourceLocation location, string functionName) => ReportError(location, DiagnosticMessage.InvalidReturnExpression(functionName));
    internal void ReportInvalidReturnExpression(SourceLocation location, string functionName, PrimType expectedType) => ReportError(location, DiagnosticMessage.InvalidReturnExpression(functionName, expectedType));
    internal void ReportNotAllPathsReturn(SourceLocation location) => ReportError(location, DiagnosticMessage.NotAllPathsReturn());
    internal void ReportUnreachableCode(SyntaxNode unreachableNode)
    {
        if (unreachableNode.NodeKind is SyntaxNodeKind.BlockExpression)
            unreachableNode = ((/*BlockExpression*/dynamic)unreachableNode).Statements.FirstOrDefault() ?? unreachableNode;
        var location = unreachableNode.FirstToken.Location;

        ReportWarning(location, DiagnosticMessage.UnreachableCode());
    }
}
