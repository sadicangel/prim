using CodeAnalysis.Syntax;
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

    public void ReportInvalidNumber(SourceLocation location, string text, PrimType type) => ReportError(location, DiagnosticMessage.InvalidNumber(text, type));
    public void ReportInvalidCharacter(SourceLocation location, char character) => ReportError(location, DiagnosticMessage.InvalidCharacter(character));
    public void ReportUnexpectedToken(TokenKind expected, Token actual) => ReportError(actual.Location, DiagnosticMessage.UnexpectedToken(expected, actual.TokenKind));
    public void ReportUndefinedUnaryOperator(Token @operator, PrimType operandType) => ReportError(@operator.Location, DiagnosticMessage.UndefinedUnaryOperator(@operator, operandType));
    public void ReportUndefinedBinaryOperator(Token @operator, PrimType leftType, PrimType rightType) => ReportError(@operator.Location, DiagnosticMessage.UndefinedBinaryOperator(@operator, leftType, rightType));
    public void ReportAmbiguousBinaryOperator(Token @operator, PrimType leftType, PrimType rightType) => ReportError(@operator.Location, DiagnosticMessage.AmbiguousBinaryOperator(@operator, leftType, rightType));
    public void ReportUndefinedName(Token identifier) => ReportError(identifier.Location, DiagnosticMessage.UndefinedName(identifier));
    public void ReportUndefinedType(SourceLocation location, string typeName) => ReportError(location, DiagnosticMessage.UndefinedType(typeName));
    public void ReportInvalidConversion(SourceLocation location, PrimType sourceType, PrimType destinationType) => ReportError(location, DiagnosticMessage.InvalidConversion(sourceType, destinationType));
    public void ReportInvalidImplicitConversion(SourceLocation location, PrimType sourceType, PrimType destinationType) => ReportError(location, DiagnosticMessage.InvalidImplicitConversion(sourceType, destinationType));
    public void ReportRedundantConversion(SourceLocation location) => ReportWarning(location, DiagnosticMessage.RedundantConversion());
    public void ReportRedeclaration(Token identifier, string type) => ReportError(identifier.Location, DiagnosticMessage.Redeclaration(identifier, type));
    public void ReportReadOnlyAssignment(SourceLocation location, string name) => ReportError(location, DiagnosticMessage.ReadOnlyAssignment(name));
    public void ReportUnterminatedString(SourceLocation location) => ReportError(location, DiagnosticMessage.UnterminatedString());
    public void ReportUnterminatedComment(SourceLocation location) => ReportError(location, DiagnosticMessage.UnterminatedComment());
    public void ReportInvalidArgumentCount(SourceLocation location, string functionName, int expectedCount, int actualCount) => ReportError(location, DiagnosticMessage.InvalidArgumentCount(functionName, expectedCount, actualCount));
    public void ReportInvalidArgumentType(SourceLocation location, string parameterName, PrimType expectedType, PrimType actualType) => ReportError(location, DiagnosticMessage.InvalidArgumentType(parameterName, expectedType, actualType));
    public void ReportInvalidExpressionType(SourceLocation location, PrimType actualType) => ReportError(location, DiagnosticMessage.InvalidExpressionType(actualType));
    public void ReportInvalidVariableType(SourceLocation location, PrimType expectedType, PrimType actualType) => ReportError(location, DiagnosticMessage.InvalidVariableType(expectedType, actualType));
    public void ReportInvalidType(SourceLocation location) => ReportError(location, DiagnosticMessage.InvalidType());
    public void ReportInvalidExpressionType(SourceLocation location, PrimType expectedType, PrimType actualType) => ReportError(location, DiagnosticMessage.InvalidExpressionType(expectedType, actualType));
    public void ReportInvalidSymbol(Token identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => ReportError(identifierToken.Location, DiagnosticMessage.InvalidSymbol(identifierToken, expectedKind, actualKind));
    public void ReportInvalidBreakOrContinue(SourceLocation location) => ReportError(location, DiagnosticMessage.InvalidBreakOrContinue());
    public void ReportInvalidReturn(SourceLocation location) => ReportError(location, DiagnosticMessage.InvalidReturn());
    public void ReportInvalidReturnExpression(SourceLocation location, string functionName) => ReportError(location, DiagnosticMessage.InvalidReturnExpression(functionName));
    public void ReportInvalidReturnExpression(SourceLocation location, string functionName, PrimType expectedType) => ReportError(location, DiagnosticMessage.InvalidReturnExpression(functionName, expectedType));
    public void ReportNotAllPathsReturn(SourceLocation location) => ReportError(location, DiagnosticMessage.NotAllPathsReturn());
    public void ReportUnreachableCode(SyntaxNode unreachableNode)
    {
        if (unreachableNode.NodeKind is SyntaxNodeKind.BlockExpression)
            unreachableNode = ((/*BlockExpression*/dynamic)unreachableNode).Statements.FirstOrDefault() ?? unreachableNode;
        var location = unreachableNode.FirstToken.Location;

        ReportWarning(location, DiagnosticMessage.UnreachableCode());
    }
}
