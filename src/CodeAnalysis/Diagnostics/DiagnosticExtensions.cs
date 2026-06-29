using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Diagnostics;

internal static class DiagnosticExtensions
{
    extension(Diagnostic)
    {
        // Scanning Errors.
        public static Diagnostic InvalidCharacter(SourceSpan sourceSpan, char character) =>
            new(DiagnosticId.InvalidCharacter, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidCharacter(character));

        public static Diagnostic InvalidSyntaxValue(SourceSpan sourceSpan, SyntaxKind kind) =>
            new(DiagnosticId.InvalidSyntaxValue, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidSyntaxValue(sourceSpan.ToString(), kind));

        public static Diagnostic UnterminatedComment(SourceSpan sourceSpan) =>
            new(DiagnosticId.UnterminatedComment, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.UnterminatedComment());

        public static Diagnostic UnterminatedString(SourceSpan sourceSpan) =>
            new(DiagnosticId.UnterminatedString, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.UnterminatedString());

        // Parsing Errors.
        public static Diagnostic ExpectedTypeDefinition(SourceSpan sourceSpan) =>
            new(DiagnosticId.ExpectedTypeDefinition, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.ExpectedTypeDefinition());

        public static Diagnostic InvalidLocationForFunctionDefinition(SourceSpan sourceSpan) =>
            new(DiagnosticId.InvalidLocationForFunctionDefinition, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidLocationForFunctionDefinition());

        public static Diagnostic InvalidLocationForTypeDefinition(SourceSpan sourceSpan) =>
            new(DiagnosticId.InvalidLocationForTypeDefinition, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidLocationForTypeDefinition());

        public static Diagnostic InvalidModuleDeclaration(SourceSpan sourceSpan) =>
            new(DiagnosticId.InvalidModuleDeclaration, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidModuleDeclaration());

        public static Diagnostic UnexpectedToken(SyntaxKind expected, SyntaxToken actual) =>
            new(DiagnosticId.UnexpectedToken, DiagnosticSeverity.Error, actual.SourceSpan, DiagnosticMessage.UnexpectedToken(expected, actual.Kind));

        // Binding Errors.
        public static Diagnostic AmbiguousSymbol(SourceSpan sourceSpan, string symbolName) =>
            new(DiagnosticId.AmbiguousSymbol, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.AmbiguousSymbol(symbolName));

        public static Diagnostic AmbiguousBinaryOperator(SyntaxToken @operator, string leftTypeName, string rightTypeName) =>
            new(DiagnosticId.AmbiguousBinaryOperator, DiagnosticSeverity.Error, @operator.SourceSpan, DiagnosticMessage.AmbiguousBinaryOperator(@operator, leftTypeName, rightTypeName));

        public static Diagnostic AmbiguousInvocationOperator(SourceSpan sourceSpan, params ReadOnlySpan<string> typeNames) =>
            new(DiagnosticId.AmbiguousInvocationOperator, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.AmbiguousInvocationOperator(typeNames));

        public static Diagnostic AmbiguousUnaryOperator(SyntaxToken @operator, string operandTypeName) =>
            new(DiagnosticId.AmbiguousUnaryOperator, DiagnosticSeverity.Error, @operator.SourceSpan, DiagnosticMessage.AmbiguousUnaryOperator(@operator, operandTypeName));

        public static Diagnostic IndexOutOfRange(SourceSpan sourceSpan, int arrayLength) =>
            new(DiagnosticId.IndexOutOfRange, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.IndexOutOfRange(arrayLength));

        public static Diagnostic InvalidParameterCount(SourceSpan sourceSpan, int expected, int actual) =>
            new(DiagnosticId.InvalidParameterCount, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidParameterCount(expected, actual));

        public static Diagnostic ArgumentCountMismatch(SourceSpan sourceSpan, int listLength) =>
            new(DiagnosticId.ArgumentCountMismatch, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.ArgumentCountMismatch(listLength));

        public static Diagnostic InvalidArrayLength(SourceSpan sourceSpan) =>
            new(DiagnosticId.InvalidArrayLength, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidArrayLength());

        public static Diagnostic ArrayElementTypeMismatch(SourceSpan sourceSpan) =>
            new(DiagnosticId.ArrayElementTypeMismatch, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.ArrayElementTypeMismatch());

        public static Diagnostic InvalidAssignment(SourceSpan sourceSpan) =>
            new(DiagnosticId.InvalidAssignment, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidAssignment());

        public static Diagnostic InvalidBreakOrContinue(SourceSpan sourceSpan) =>
            new(DiagnosticId.InvalidBreakOrContinue, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidBreakOrContinue());

        public static Diagnostic InvalidConversion(SourceSpan sourceSpan, string sourceTypeName, string targetTypeName) =>
            new(DiagnosticId.InvalidConversion, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidConversion(sourceTypeName, targetTypeName));

        public static Diagnostic InvalidConversionDeclaration(SourceSpan sourceSpan) =>
            new(DiagnosticId.InvalidConversionDeclaration, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidConversionDeclaration());

        public static Diagnostic InvalidExpressionType(SourceSpan sourceSpan, string expectedTypeName, string actualTypeName) =>
            new(DiagnosticId.InvalidExpressionType, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidExpressionType(expectedTypeName, actualTypeName));

        public static Diagnostic InvalidImplicitConversion(SourceSpan sourceSpan, string sourceTypeName, string targetTypeName) =>
            new(DiagnosticId.InvalidImplicitConversion, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidImplicitConversion(sourceTypeName, targetTypeName));

        public static Diagnostic InvalidImplicitType(SourceSpan sourceSpan, string typeName) =>
            new(DiagnosticId.InvalidImplicitType, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidImplicitType(typeName));

        public static Diagnostic InvalidOperatorDeclaration(SourceSpan sourceSpan, string operatorKind, string operationParameterCount) =>
            new(DiagnosticId.InvalidOperatorDeclaration, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidOperatorDeclaration(operatorKind, operationParameterCount));

        public static Diagnostic InvalidQualifiedDeclarationName(SourceSpan sourceSpan) =>
            new(DiagnosticId.InvalidQualifiedDeclarationName, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidQualifiedDeclarationName());

        public static Diagnostic InvalidModulePath(SourceSpan sourceSpan, string modulePath) =>
            new(DiagnosticId.InvalidModulePath, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidModulePath(modulePath));

        public static Diagnostic InvalidReturn(SourceSpan sourceSpan) =>
            new(DiagnosticId.InvalidReturn, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.InvalidReturn());

        public static Diagnostic ReadOnlyAssignment(SourceSpan sourceSpan, string symbolName) =>
            new(DiagnosticId.ReadOnlyAssignment, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.ReadOnlyAssignment(symbolName));

        public static Diagnostic RedundantConversion(SourceSpan sourceSpan) =>
            new(DiagnosticId.RedundantConversion, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.RedundantConversion());

        public static Diagnostic SymbolRedeclaration(SourceSpan sourceSpan, string symbolName) =>
            new(DiagnosticId.SymbolRedeclaration, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.SymbolRedeclaration(symbolName));

        public static Diagnostic UndefinedBinaryOperator(SyntaxToken @operator, string leftTypeName, string rightTypeName) =>
            new(DiagnosticId.UndefinedBinaryOperator, DiagnosticSeverity.Error, @operator.SourceSpan, DiagnosticMessage.UndefinedBinaryOperator(@operator, leftTypeName, rightTypeName));

        public static Diagnostic UndefinedIndexOperator(SourceSpan sourceSpan, string containingTypeName) =>
            new(DiagnosticId.UndefinedIndexOperator, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.UndefinedInvocationOperator(containingTypeName));

        public static Diagnostic UndefinedInvocationOperator(SourceSpan sourceSpan, string containingTypeName) =>
            new(DiagnosticId.UndefinedInvocationOperator, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.UndefinedInvocationOperator(containingTypeName));

        public static Diagnostic UndefinedType(SourceSpan sourceSpan, string typeName) =>
            new(DiagnosticId.UndefinedType, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.UndefinedType(typeName));

        public static Diagnostic UndefinedTypeMember(SourceSpan sourceSpan, string typeName, string memberName) =>
            new(DiagnosticId.UndefinedTypeMember, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.UndefinedTypeMember(typeName, memberName));

        public static Diagnostic UndefinedSymbol(SourceSpan sourceSpan, string symbolName) =>
            new(DiagnosticId.UndefinedSymbol, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.UndefinedSymbol(symbolName));

        public static Diagnostic UndefinedUnaryOperator(SyntaxToken @operator, string operandTypeName) =>
            new(DiagnosticId.UndefinedUnaryOperator, DiagnosticSeverity.Error, @operator.SourceSpan, DiagnosticMessage.UndefinedUnaryOperator(@operator, operandTypeName));

        public static Diagnostic UninitializedProperty(SourceSpan sourceSpan, string propertyName) =>
            new(DiagnosticId.UninitializedProperty, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.UninitializedProperty(propertyName));

        public static Diagnostic UninitializedVariable(SourceSpan sourceSpan, string variableName) =>
            new(DiagnosticId.UninitializedVariable, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.UninitializedVariable(variableName));

        public static Diagnostic UnreachableCode(SourceSpan sourceSpan) =>
            new(DiagnosticId.UnreachableCode, DiagnosticSeverity.Error, sourceSpan, DiagnosticMessage.UnreachableCode());

        //public void ReportInvalidArgumentType(SourceSpan sourceSpan, Parameter parameter, PrimType actualType) => reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidArgumentType(parameter, actualType));
        //public void ReportInvalidExpressionType(SourceSpan sourceSpan, PrimType expectedType, PrimType actualType) => reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidExpressionType(expectedType, actualType));
        //public void ReportInvalidSymbol(SyntaxToken identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => reporter.ReportError(identifierToken.SourceSpan, DiagnosticMessage.InvalidSymbol(identifierToken, expectedKind, actualKind));
        //public void ReportInvalidReturnExpression(SourceSpan sourceSpan, string functionName) => reporter.ReportError(sourceSpan, DiagnosticMessage.InvalidReturnExpression(functionName));
        //public void ReportNotAllPathsReturn(SourceSpan sourceSpan) => reporter.ReportError(sourceSpan, DiagnosticMessage.NotAllPathsReturn());
    }
}
