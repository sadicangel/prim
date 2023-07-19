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

    public void ReportError(TextLocation location, string message) => _diagnostics.Add(new Diagnostic(IsError: true, location, message));
    public void ReportWarning(TextLocation location, string message) => _diagnostics.Add(new Diagnostic(IsError: false, location, message));

    public void ReportInvalidNumber(TextLocation location, string text, TypeSymbol type) => ReportError(location, DiagnosticMessage.InvalidNumber(text, type));
    public void ReportInvalidCharacter(TextLocation location, char character) => ReportError(location, DiagnosticMessage.InvalidCharacter(character));
    public void ReportUnexpectedToken(TokenKind expected, Token actual) => ReportError(actual.GetLocation(), DiagnosticMessage.UnexpectedToken(expected, actual.TokenKind));
    public void ReportUndefinedUnaryOperator(Token @operator, TypeSymbol operandType) => ReportError(@operator.GetLocation(), DiagnosticMessage.UndefinedUnaryOperator(@operator.Text, operandType));
    public void ReportUndefinedBinaryOperator(Token @operator, TypeSymbol leftType, TypeSymbol rightType) => ReportError(@operator.GetLocation(), DiagnosticMessage.UndefinedBinaryOperator(@operator.Text, leftType, rightType));
    public void ReportUndefinedName(Token identifier) => ReportError(identifier.GetLocation(), DiagnosticMessage.UndefinedName(identifier.Text));
    public void ReportUndefinedType(TextLocation location, string typeName) => ReportError(location, DiagnosticMessage.UndefinedType(typeName));
    public void ReportInvalidConversion(TextLocation location, TypeSymbol sourceType, TypeSymbol destinationType) => ReportError(location, DiagnosticMessage.InvalidConversion(sourceType, destinationType));
    public void ReportInvalidImplicitConversion(TextLocation location, TypeSymbol sourceType, TypeSymbol destinationType) => ReportError(location, DiagnosticMessage.InvalidImplicitConversion(sourceType, destinationType));
    public void ReportRedeclaration(Token identifier, string type) => ReportError(identifier.GetLocation(), DiagnosticMessage.Redeclaration(identifier.Text, type));
    public void ReportReadOnlyAssignment(TextLocation location, string name) => ReportError(location, DiagnosticMessage.ReadOnlyAssignment(name));
    public void ReportUnterminatedString(TextLocation location) => ReportError(location, DiagnosticMessage.UnterminatedString());
    public void ReportUnterminatedComment(TextLocation location) => ReportError(location, DiagnosticMessage.UnterminatedComment());
    public void ReportInvalidArgumentCount(TextLocation location, string functionName, int expectedCount, int actualCount) => ReportError(location, DiagnosticMessage.InvalidArgumentCount(functionName, expectedCount, actualCount));
    public void ReportInvalidArgumentType(TextLocation location, string parameterName, TypeSymbol expectedType, TypeSymbol actualType) => ReportError(location, DiagnosticMessage.InvalidArgumentType(parameterName, expectedType, actualType));
    public void ReportInvalidExpressionType(TextLocation location, TypeSymbol actualType) => ReportError(location, DiagnosticMessage.InvalidExpressionType(actualType));
    public void ReportInvalidVariableType(TextLocation location, TypeSymbol expectedType, TypeSymbol actualType) => ReportError(location, DiagnosticMessage.InvalidVariableType(expectedType, actualType));
    public void ReportInvalidExpressionType(TextLocation location, TypeSymbol expectedType, TypeSymbol actualType) => ReportError(location, DiagnosticMessage.InvalidExpressionType(expectedType, actualType));
    public void ReportInvalidSymbol(Token identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => ReportError(identifierToken.GetLocation(), DiagnosticMessage.InvalidSymbol(identifierToken, expectedKind, actualKind));
    public void ReportInvalidBreakOrContinue(TextLocation location) => ReportError(location, DiagnosticMessage.InvalidBreakOrContinue());
    public void ReportInvalidReturn(TextLocation location) => ReportError(location, DiagnosticMessage.InvalidReturn());
    public void ReportInvalidReturnExpression(TextLocation location, string functionName) => ReportError(location, DiagnosticMessage.InvalidReturnExpression(functionName));
    public void ReportInvalidReturnExpression(TextLocation location, string functionName, TypeSymbol expectedType) => ReportError(location, DiagnosticMessage.InvalidReturnExpression(functionName, expectedType));
    public void ReportNotAllPathsReturn(TextLocation location) => ReportError(location, DiagnosticMessage.NotAllPathsReturn());
}
