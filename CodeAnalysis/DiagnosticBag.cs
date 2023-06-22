using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using System.Collections;

namespace CodeAnalysis;

public sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics = new();

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddRange(IEnumerable<Diagnostic> diagnostics) => _diagnostics.AddRange(diagnostics);

    public void ReportError(TextSpan span, string message) => _diagnostics.Add(new Diagnostic(IsError: true, span, message));
    public void ReportWarning(TextSpan span, string message) => _diagnostics.Add(new Diagnostic(IsError: false, span, message));

    public void ReportInvalidNumber(TextSpan span, string text, TypeSymbol type) => ReportError(span, DiagnosticMessage.InvalidNumber(text, type));
    public void ReportInvalidCharacter(int position, char character) => ReportError(new TextSpan(position, 1), DiagnosticMessage.InvalidCharacter(character));
    public void ReportUnexpectedToken(TokenKind expected, Token actual) => ReportError(actual.Span, DiagnosticMessage.UnexpectedToken(expected, actual.Kind));
    public void ReportUndefinedUnaryOperator(Token @operator, TypeSymbol operandType) => ReportError(@operator.Span, DiagnosticMessage.UndefinedUnaryOperator(@operator.Text, operandType));
    public void ReportUndefinedBinaryOperator(Token @operator, TypeSymbol leftType, TypeSymbol rightType) => ReportError(@operator.Span, DiagnosticMessage.UndefinedBinaryOperator(@operator.Text, leftType, rightType));
    public void ReportUndefinedName(Token identifier) => ReportError(identifier.Span, DiagnosticMessage.UndefinedName(identifier.Text));
    public void ReportInvalidConversion(TextSpan span, TypeSymbol sourceType, TypeSymbol destinationType) => ReportError(span, DiagnosticMessage.InvalidConversion(sourceType, destinationType));
    public void ReportRedeclaration(Token identifier) => ReportError(identifier.Span, DiagnosticMessage.Redeclaration(identifier.Text));
    public void ReportReadOnlyAssignment(TextSpan span, string name) => ReportError(span, DiagnosticMessage.ReadOnlyAssignment(name));
    internal void ReportUnterminatedString(TextSpan span) => ReportError(span, DiagnosticMessage.UnterminatedString());
    internal void ReportInvalidArgumentCount(TextSpan span, string functionName, int expectedCount, int actualCount) => ReportError(span, DiagnosticMessage.InvalidArgumentCount(functionName, expectedCount, actualCount));
    internal void ReportInvalidArgumentType(TextSpan span, string parameterName, TypeSymbol expectedType, TypeSymbol actualType) => ReportError(span, DiagnosticMessage.InvalidArgumentType(parameterName, expectedType, actualType));
    internal void ReportInvalidExpressionType(TextSpan span, TypeSymbol type) => ReportError(span, DiagnosticMessage.InvalidExpressionType(type));
}