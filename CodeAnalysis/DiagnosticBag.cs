using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using System.Collections;

namespace CodeAnalysis;

public sealed class DiagnosticBag : IReadOnlyList<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics;

    public int Count { get => _diagnostics.Count; }

    public Diagnostic this[int index] { get => _diagnostics[index]; }

    public DiagnosticBag() => _diagnostics = new();

    public DiagnosticBag(IEnumerable<Diagnostic> diagnostics) => _diagnostics = new(diagnostics);

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddRange(IEnumerable<Diagnostic> diagnostics) => _diagnostics.AddRange(diagnostics);

    public void ReportError(TextSpan span, string message) => _diagnostics.Add(new Diagnostic(IsError: true, span, message));
    public void ReportWarning(TextSpan span, string message) => _diagnostics.Add(new Diagnostic(IsError: false, span, message));

    public void ReportInvalidNumber(TextSpan span, string text, TypeSymbol type) => ReportError(span, DiagnosticMessage.InvalidNumber(text, type));
    public void ReportInvalidCharacter(int position, char character) => ReportError(new TextSpan(position, 1), DiagnosticMessage.InvalidCharacter(character));
    public void ReportUnexpectedToken(TokenKind expected, Token actual) => ReportError(actual.Span, DiagnosticMessage.UnexpectedToken(expected, actual.TokenKind));
    public void ReportUndefinedUnaryOperator(Token @operator, TypeSymbol operandType) => ReportError(@operator.Span, DiagnosticMessage.UndefinedUnaryOperator(@operator.Text, operandType));
    public void ReportUndefinedBinaryOperator(Token @operator, TypeSymbol leftType, TypeSymbol rightType) => ReportError(@operator.Span, DiagnosticMessage.UndefinedBinaryOperator(@operator.Text, leftType, rightType));
    public void ReportUndefinedName(Token identifier) => ReportError(identifier.Span, DiagnosticMessage.UndefinedName(identifier.Text));
    public void ReportUndefinedType(TextSpan span, string typeName) => ReportError(span, DiagnosticMessage.UndefinedType(typeName));
    public void ReportInvalidConversion(TextSpan span, TypeSymbol sourceType, TypeSymbol destinationType) => ReportError(span, DiagnosticMessage.InvalidConversion(sourceType, destinationType));
    public void ReportInvalidImplicitConversion(TextSpan span, TypeSymbol sourceType, TypeSymbol destinationType) => ReportError(span, DiagnosticMessage.InvalidImplicitConversion(sourceType, destinationType));
    public void ReportRedeclaration(Token identifier, string type) => ReportError(identifier.Span, DiagnosticMessage.Redeclaration(identifier.Text, type));
    public void ReportReadOnlyAssignment(TextSpan span, string name) => ReportError(span, DiagnosticMessage.ReadOnlyAssignment(name));
    public void ReportUnterminatedString(TextSpan span) => ReportError(span, DiagnosticMessage.UnterminatedString());
    public void ReportInvalidArgumentCount(TextSpan span, string functionName, int expectedCount, int actualCount) => ReportError(span, DiagnosticMessage.InvalidArgumentCount(functionName, expectedCount, actualCount));
    public void ReportInvalidArgumentType(TextSpan span, string parameterName, TypeSymbol expectedType, TypeSymbol actualType) => ReportError(span, DiagnosticMessage.InvalidArgumentType(parameterName, expectedType, actualType));
    public void ReportInvalidExpressionType(TextSpan span, TypeSymbol actualType) => ReportError(span, DiagnosticMessage.InvalidExpressionType(actualType));
    public void ReportInvalidVariableType(TextSpan span, TypeSymbol expectedType, TypeSymbol actualType) => ReportError(span, DiagnosticMessage.InvalidVariableType(expectedType, actualType));
    public void ReportInvalidExpressionType(TextSpan span, TypeSymbol expectedType, TypeSymbol actualType) => ReportError(span, DiagnosticMessage.InvalidExpressionType(expectedType, actualType));
    public void ReportInvalidSymbol(Token identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => ReportError(identifierToken.Span, DiagnosticMessage.InvalidSymbol(identifierToken, expectedKind, actualKind));
    public void ReportInvalidBreakOrContinue(TextSpan span) => ReportError(span, DiagnosticMessage.InvalidBreakOrContinue());
    public void ReportInvalidReturn(TextSpan span) => ReportError(span, DiagnosticMessage.InvalidReturn());
    public void ReportInvalidReturnExpression(TextSpan span, string functionName) => ReportError(span, DiagnosticMessage.InvalidReturnExpression(functionName));
    public void ReportInvalidReturnExpression(TextSpan span, string functionName, TypeSymbol expectedType) => ReportError(span, DiagnosticMessage.InvalidReturnExpression(functionName, expectedType));
}