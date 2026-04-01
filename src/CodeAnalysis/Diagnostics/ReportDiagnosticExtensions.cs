using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Diagnostics;

internal static class ReportDiagnosticExtensions
{
    extension(IDiagnosticReporter reporter)
    {
        public void ReportError(SourceSpan sourceSpan, string message) =>
            reporter.Report(sourceSpan, DiagnosticSeverity.Error, message);

        public void ReportWarning(SourceSpan sourceSpan, string message) =>
            reporter.Report(sourceSpan, DiagnosticSeverity.Warning, message);

        public void ReportInformation(SourceSpan sourceSpan, string message) =>
            reporter.Report(sourceSpan, DiagnosticSeverity.Warning, message);

        // Scanning Errors.
        public void ReportInvalidCharacter(SourceSpan sourceSpan, char character) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidCharacter(character));

        public void ReportInvalidSyntaxValue(SourceSpan sourceSpan, SyntaxKind kind) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidSyntaxValue(sourceSpan.ToString(), kind));

        public void ReportUnterminatedComment(SourceSpan sourceSpan) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.UnterminatedComment());

        public void ReportUnterminatedString(SourceSpan sourceSpan) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.UnterminatedString());

        // Parsing Errors.
        public void ReportExpectedTypeDefinition(SourceSpan sourceSpan) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.ExpectedTypeDefinition());

        public void ReportInvalidLocationForFunctionDefinition(SourceSpan sourceSpan) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidLocationForFunctionDefinition());

        public void ReportInvalidLocationForTypeDefinition(SourceSpan sourceSpan) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidLocationForTypeDefinition());

        public void ReportUnexpectedToken(SyntaxKind expected, SyntaxToken actual) =>
            reporter.ReportError(actual.SourceSpan, DiagnosticMessage.UnexpectedToken(expected, actual.SyntaxKind));

        // Binding Errors.
        public void ReportAmbiguousBinaryOperator(SyntaxToken @operator, string leftTypeName, string rightTypeName) =>
            reporter.ReportError(@operator.SourceSpan, DiagnosticMessage.AmbiguousBinaryOperator(@operator, leftTypeName, rightTypeName));

        public void ReportAmbiguousInvocationOperator(SourceSpan sourceSpan, params ReadOnlySpan<string> typeNames) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.AmbiguousInvocationOperator(typeNames));

        public void ReportAmbiguousUnaryOperator(SyntaxToken @operator, string operandTypeName) =>
            reporter.ReportError(@operator.SourceSpan, DiagnosticMessage.AmbiguousUnaryOperator(@operator, operandTypeName));

        public void ReportIndexOutOfRange(SourceSpan sourceSpan, int arrayLength) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.IndexOutOfRange(arrayLength));

        public void ReportInvalidParameterCount(SourceSpan sourceSpan, int expected, int actual) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidParameterCount(expected, actual));

        public void ReportArgumentCountMismatch(SourceSpan sourceSpan, int listLength) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.ArgumentCountMismatch(listLength));

        public void ReportInvalidArrayLength(SourceSpan sourceSpan) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidArrayLength());

        public void ReportInvalidAssignment(SourceSpan sourceSpan) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidAssignment());

        public void ReportInvalidBreakOrContinue(SourceSpan sourceSpan) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidBreakOrContinue());

        public void ReportInvalidConversion(SourceSpan sourceSpan, string sourceTypeName, string targetTypeName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidConversion(sourceTypeName, targetTypeName));

        public void ReportInvalidConversionDeclaration(SourceSpan sourceSpan) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidConversionDeclaration());

        public void ReportInvalidExpressionType(SourceSpan sourceSpan, string expectedTypeName, string actualTypeName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidExpressionType(expectedTypeName, actualTypeName));

        public void ReportInvalidImplicitConversion(SourceSpan sourceSpan, string sourceTypeName, string targetTypeName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidImplicitConversion(sourceTypeName, targetTypeName));

        public void ReportInvalidImplicitType(SourceSpan sourceSpan, string typeName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidImplicitType(typeName));

        public void ReportInvalidOperatorDeclaration(SourceSpan sourceSpan, string operatorKind, string operationParameterCount) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidOperatorDeclaration(operatorKind, operationParameterCount));

        public void ReportInvalidReturn(SourceSpan sourceSpan) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidReturn());

        public void ReportReadOnlyAssignment(SourceSpan sourceSpan, string symbolName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.ReadOnlyAssignment(symbolName));

        public void ReportRedundantConversion(SourceSpan sourceSpan) =>
            reporter.ReportWarning(sourceSpan, DiagnosticMessage.RedundantConversion());

        public void ReportSymbolRedeclaration(SourceSpan sourceSpan, string symbolName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.SymbolRedeclaration(symbolName));

        public void ReportUndefinedBinaryOperator(SyntaxToken @operator, string leftTypeName, string rightTypeName) =>
            reporter.ReportError(@operator.SourceSpan, DiagnosticMessage.UndefinedBinaryOperator(@operator, leftTypeName, rightTypeName));

        public void ReportUndefinedIndexOperator(SourceSpan sourceSpan, string containingTypeName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.UndefinedInvocationOperator(containingTypeName));

        public void ReportUndefinedInvocationOperator(SourceSpan sourceSpan, string containingTypeName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.UndefinedInvocationOperator(containingTypeName));

        public void ReportUndefinedType(SourceSpan sourceSpan, string typeName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.UndefinedType(typeName));

        public void ReportUndefinedTypeMember(SourceSpan sourceSpan, string typeName, string memberName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.UndefinedTypeMember(typeName, memberName));

        public void ReportUndefinedSymbol(SourceSpan sourceSpan, string symbolName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.UndefinedSymbol(symbolName));

        public void ReportUndefinedUnaryOperator(SyntaxToken @operator, string operandTypeName) =>
            reporter.ReportError(@operator.SourceSpan, DiagnosticMessage.UndefinedUnaryOperator(@operator, operandTypeName));

        public void ReportUninitializedProperty(SourceSpan sourceSpan, string propertyName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.UninitializedProperty(propertyName));

        public void ReportUninitializedVariable(SourceSpan sourceSpan, string variableName) =>
            reporter.ReportError(sourceSpan, DiagnosticMessage.UninitializedVariable(variableName));

        public void ReportUnreachableCode(SourceSpan sourceSpan) =>
            reporter.ReportWarning(sourceSpan, DiagnosticMessage.UnreachableCode());


        //public void ReportInvalidArgumentType(SourceSpan sourceSpan, Parameter parameter, PrimType actualType) => reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidArgumentType(parameter, actualType));
        //public void ReportInvalidExpressionType(SourceSpan sourceSpan, PrimType expectedType, PrimType actualType) => reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidExpressionType(expectedType, actualType));
        //public void ReportInvalidSymbol(SyntaxToken identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => reporter.ReportError(identifierToken.SourceSpan, DiagnosticMessage.InvalidSymbol(identifierToken, expectedKind, actualKind));
        //public void ReportInvalidReturnExpression(SourceSpan sourceSpan, string functionName) => reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidReturnExpression(functionName));
        //public void ReportNotAllPathsReturn(SourceSpan sourceSpan) => reporter.ReportError(sourceSpan, DiagnosticMessage.NotAllPathsReturn());
    }
}
