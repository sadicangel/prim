using System.Collections;
using System.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Diagnostics;

public readonly record struct SymbolKind;

public interface IReadOnlyDiagnosticBag : IReadOnlyList<Diagnostic>
{
    public bool HasErrorDiagnostics { get; }
    public bool HasWarningDiagnostics { get; }

    public IEnumerable<Diagnostic> GetErrors() => this.Where(d => d.Severity is DiagnosticSeverity.Error);
    public IEnumerable<Diagnostic> GetWarnings() => this.Where(d => d.Severity is DiagnosticSeverity.Warning);
}

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
public sealed class DiagnosticBag : IReadOnlyDiagnosticBag
{
    private readonly List<Diagnostic> _diagnostics;

    public DiagnosticBag() => _diagnostics = [];
    public DiagnosticBag(IEnumerable<Diagnostic> diagnostics) => _diagnostics = new(diagnostics);

    public int Count { get => _diagnostics.Count; }

    public bool HasErrorDiagnostics { get => _diagnostics.Any(d => d.Severity is DiagnosticSeverity.Error); }

    public bool HasWarningDiagnostics { get => _diagnostics.Any(d => d.Severity is DiagnosticSeverity.Warning); }

    public bool HasInformationDiagnostics { get => _diagnostics.Any(d => d.Severity is DiagnosticSeverity.Information); }

    public Diagnostic this[int index] { get => _diagnostics[index]; }

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddRange(IEnumerable<Diagnostic> diagnostics) => _diagnostics.AddRange(diagnostics);

    public void Report(SourceLocation location, DiagnosticSeverity severity, string message)
    {
        _diagnostics.Add(new Diagnostic(Id: "", location, severity, message));
    }

    public void ReportError(SourceLocation location, string message) =>
        Report(location, DiagnosticSeverity.Error, message);
    public void ReportWarning(SourceLocation location, string message) =>
        Report(location, DiagnosticSeverity.Warning, message);
    public void ReportInformation(SourceLocation location, string message) =>
        Report(location, DiagnosticSeverity.Warning, message);

    // Scanning Errors.
    internal void ReportInvalidCharacter(SourceLocation location, char character) =>
        ReportError(location, DiagnosticMessage.InvalidCharacter(character));
    internal void ReportInvalidSyntaxValue(SourceLocation location, SyntaxKind kind) =>
        ReportError(location, DiagnosticMessage.InvalidSyntaxValue(location.Text.ToString(), kind));
    internal void ReportUnterminatedComment(SourceLocation location) =>
        ReportError(location, DiagnosticMessage.UnterminatedComment());
    internal void ReportUnterminatedString(SourceLocation location) =>
        ReportError(location, DiagnosticMessage.UnterminatedString());

    // Parsing Errors.
    internal void ReportExpectedTypeDefinition(SourceLocation location) =>
        ReportError(location, DiagnosticMessage.ExpectedTypeDefinition());
    internal void ReportInvalidLocationForFunctionDefinition(SourceLocation location) =>
        ReportError(location, DiagnosticMessage.InvalidLocationForFunctionDefinition());
    internal void ReportInvalidLocationForTypeDefinition(SourceLocation location) =>
        ReportError(location, DiagnosticMessage.InvalidLocationForTypeDefinition());
    internal void ReportUnexpectedToken(SyntaxKind expected, SyntaxToken actual) =>
        ReportError(actual.Location, DiagnosticMessage.UnexpectedToken(expected, actual.SyntaxKind));

    // Binding Errors.
    internal void ReportAmbiguousBinaryOperator(SyntaxToken @operator, string leftTypeName, string rightTypeName) =>
        ReportError(@operator.Location, DiagnosticMessage.AmbiguousBinaryOperator(@operator, leftTypeName, rightTypeName));
    internal void ReportAmbiguousInvocationOperator(SourceLocation location, params ReadOnlySpan<string> typeNames) =>
        ReportError(location, DiagnosticMessage.AmbiguousInvocationOperator(typeNames));
    internal void ReportAmbiguousUnaryOperator(SyntaxToken @operator, string operandTypeName) =>
        ReportError(@operator.Location, DiagnosticMessage.AmbiguousUnaryOperator(@operator, operandTypeName));
    internal void ReportIndexOutOfRange(SourceLocation location, int arrayLength) =>
        ReportError(location, DiagnosticMessage.IndexOutOfRange(arrayLength));
    internal void ReportInvalidArgumentListLength(SourceLocation location, int listLength) =>
        ReportError(location, DiagnosticMessage.InvalidArgumentListLength(listLength));
    internal void ReportInvalidArrayLength(SourceLocation location) =>
        ReportError(location, DiagnosticMessage.InvalidArrayLength());
    internal void ReportInvalidAssignment(SourceLocation location) =>
        ReportError(location, DiagnosticMessage.InvalidAssignment());
    internal void ReportInvalidConversion(SourceLocation location, string sourceTypeName, string targetTypeName) =>
        ReportError(location, DiagnosticMessage.InvalidConversion(sourceTypeName, targetTypeName));
    internal void ReportInvalidConversionDeclaration(SourceLocation location) =>
        ReportError(location, DiagnosticMessage.InvalidConversionDeclaration());
    internal void ReportInvalidExpressionType(SourceLocation location, string expectedTypeName, string actualTypeName) =>
        ReportError(location, DiagnosticMessage.InvalidExpressionType(expectedTypeName, actualTypeName));
    internal void ReportInvalidImplicitConversion(SourceLocation location, string sourceTypeName, string targetTypeName) =>
        ReportError(location, DiagnosticMessage.InvalidImplicitConversion(sourceTypeName, targetTypeName));
    internal void ReportInvalidImplicitType(SourceLocation location, string typeName) =>
        ReportError(location, DiagnosticMessage.InvalidImplicitType(typeName));
    internal void ReportInvalidOperatorDeclaration(SourceLocation location, string operatorKind, string operationParameterCount) =>
        ReportError(location, DiagnosticMessage.InvalidOperatorDeclaration(operatorKind, operationParameterCount));
    internal void ReportReadOnlyAssignment(SourceLocation location, string symbolName) =>
        ReportError(location, DiagnosticMessage.ReadOnlyAssignment(symbolName));
    internal void ReportRedundantConversion(SourceLocation location) =>
        ReportWarning(location, DiagnosticMessage.RedundantConversion());
    internal void ReportSymbolRedeclaration(SourceLocation location, string symbolName) =>
        ReportError(location, DiagnosticMessage.SymbolRedeclaration(symbolName));
    internal void ReportUndefinedBinaryOperator(SyntaxToken @operator, string leftTypeName, string rightTypeName) =>
        ReportError(@operator.Location, DiagnosticMessage.UndefinedBinaryOperator(@operator, leftTypeName, rightTypeName));
    internal void ReportUndefinedIndexOperator(SourceLocation location, string containingTypeName) =>
        ReportError(location, DiagnosticMessage.UndefinedInvocationOperator(containingTypeName));
    internal void ReportUndefinedInvocationOperator(SourceLocation location, string containingTypeName) =>
        ReportError(location, DiagnosticMessage.UndefinedInvocationOperator(containingTypeName));
    internal void ReportUndefinedType(SourceLocation location, string typeName) => ReportError(location, DiagnosticMessage.UndefinedType(typeName));
    internal void ReportUndefinedTypeMember(SourceLocation location, string typeName, string memberName) =>
        ReportError(location, DiagnosticMessage.UndefinedTypeMember(typeName, memberName));
    internal void ReportUndefinedSymbol(SourceLocation location, string symbolName) =>
        ReportError(location, DiagnosticMessage.UndefinedSymbol(symbolName));
    internal void ReportUndefinedUnaryOperator(SyntaxToken @operator, string operandTypeName) =>
        ReportError(@operator.Location, DiagnosticMessage.UndefinedUnaryOperator(@operator, operandTypeName));
    internal void ReportUninitializedProperty(SourceLocation location, string propertyName) =>
        ReportError(location, DiagnosticMessage.UninitializedProperty(propertyName));
    internal void ReportUninitializedVariable(SourceLocation location, string variableName) =>
        ReportError(location, DiagnosticMessage.UninitializedVariable(variableName));
    internal void ReportUnreachableCode(SourceLocation location) =>
        ReportWarning(location, DiagnosticMessage.UnreachableCode());

    //internal void ReportInvalidArgumentType(SourceLocation location, Parameter parameter, PrimType actualType) => ReportError(location, DiagnosticMessage.InvalidArgumentType(parameter, actualType));
    //internal void ReportInvalidExpressionType(SourceLocation location, PrimType expectedType, PrimType actualType) => ReportError(location, DiagnosticMessage.InvalidExpressionType(expectedType, actualType));
    //internal void ReportInvalidSymbol(SyntaxToken identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => ReportError(identifierToken.Location, DiagnosticMessage.InvalidSymbol(identifierToken, expectedKind, actualKind));
    //internal void ReportInvalidBreakOrContinue(SourceLocation location) => ReportError(location, DiagnosticMessage.InvalidBreakOrContinue());
    //internal void ReportInvalidReturn(SourceLocation location) => ReportError(location, DiagnosticMessage.InvalidReturn());
    //internal void ReportInvalidReturnExpression(SourceLocation location, string functionName) => ReportError(location, DiagnosticMessage.InvalidReturnExpression(functionName));
    //internal void ReportNotAllPathsReturn(SourceLocation location) => ReportError(location, DiagnosticMessage.NotAllPathsReturn());
}
