using CodeAnalysis.Text;
using System.Collections;

namespace CodeAnalysis.Syntax;

public sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics = new();

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddRange(IEnumerable<Diagnostic> diagnostics) => _diagnostics.AddRange(diagnostics);

    public void ReportError(TextSpan span, string message) => _diagnostics.Add(new Diagnostic(IsError: true, span, message));

    public void ReportWarning(TextSpan span, string message) => _diagnostics.Add(new Diagnostic(IsError: false, span, message));

    public void ReportInvalidNumber(TextSpan span, string text, Type type) => ReportError(span, $"The value {text} is not a valid {type.Name}");

    public void ReportInvalidCharacter(int position, char character) => ReportError(new TextSpan(position, 1), $"Invalid character input: {character}");

    public void ReportUnexpectedToken(TokenKind expected, Token actual) => ReportError(actual.Span, $"Unexpected token <{actual.Kind}>. Expected <{expected}>");

    public void ReportUndefinedUnaryOperator(Token @operator, Type operandType) => ReportError(@operator.Span, $"Unary operator '{@operator.Text}' is not defined for type {operandType.Name}");
    public void ReportUndefinedBinaryOperator(Token @operator, Type leftType, Type rightType) => ReportError(@operator.Span, $"Binary operator '{@operator.Text}' is not defined for types {leftType.Name} and {rightType.Name}");
    public void ReportUndefinedName(Token identifier) => ReportError(identifier.Span, $"Variable '{identifier.Text}' does not exist");
    public void ReportInvalidConversion(TextSpan span, Type sourceType, Type destinationType) => ReportError(span, $"Cannot convert from type {sourceType} to {destinationType}");
}
