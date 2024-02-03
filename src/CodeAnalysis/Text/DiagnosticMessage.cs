using CodeAnalysis.Binding;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Text;

internal static class DiagnosticMessage
{
    // Scanning error messages.
    public static string InvalidCharacter(char character) => $"Invalid character input: '{character}'";
    public static string InvalidNumber(string value, PrimType type) => $"The value '{value}' is not a valid '{type.Name}'";
    public static string UnterminatedComment() => "Unterminated comment";
    public static string UnterminatedString() => "Unterminated string literal";

    // Parsing error messages.
    public static string ExpectedTypeDefinition() => $"Expected type definition";
    public static string UnexpectedToken(TokenKind expected, TokenKind actual) => $"Unexpected token '{actual.GetDiagnosticDisplay()}'. Expected '{expected.GetDiagnosticDisplay()}'";

    // Binding error messages.
    public static string AmbiguousBinaryOperator(Token @operator, PrimType leftType, PrimType rightType) => $"Binary operator '{@operator}' is ambiguous on operands of type '{leftType}' and '{rightType}'";
    public static string InvalidExpressionType(PrimType expectedType, PrimType actualType) => $"Invalid expression of type '{actualType.Name}'. Expected '{expectedType.Name}'";
    public static string InvalidTypeConversion(PrimType sourceType, PrimType targetType) => $"Invalid conversion from type '{sourceType.Name}' to '{targetType.Name}'";
    public static string SymbolReassignment(Symbol symbol) => $"Reassignment of read-only symbol '{symbol.Name}'";
    public static string SymbolRedeclaration(Symbol symbol) => $"Redeclaration of symbol '{symbol.Name}'";
    public static string UndefinedBinaryOperator(Token @operator, PrimType leftType, PrimType rightType) => $"Binary operator '{@operator}' is not defined for types '{leftType}' and '{rightType}'";
    public static string UndefinedType(PrimType type) => $"Undefined type '{type.Name}'";
    public static string ReportUndefinedSymbol(Symbol symbol) => $"Undefined symbol '{symbol.Name}'";
    public static string UndefinedUnaryOperator(Token @operator, PrimType operandType) => $"Unary operator '{@operator}' is not defined for type '{operandType}'";



    public static string InvalidImplicitConversion(PrimType sourceType, PrimType destinationType) => $"Invalid implicit conversion from type '{sourceType}' to '{destinationType}'. An explicit conversion exists (are you missing a cast?)";

    public static string RedundantConversion() => "Conversion is redundant";

    public static string InvalidArgumentCount(string functionName, int expectedCount, int actualCount) => $"Function '{functionName}' requires {expectedCount} arguments but was given {actualCount}";

    public static string InvalidArgumentType(string parameterName, PrimType expectedType, PrimType actualType) => $"Parameter '{parameterName}' cannot be converted from type '{actualType}' to '{expectedType}'";

    public static string InvalidExpressionType(PrimType actualType) => $"Invalid expression of type '{actualType}'";

    public static string InvalidVariableType(PrimType expectedType, PrimType actualType) => $"Expected variable of type '{expectedType}'. Got '{actualType}'";

    public static string InvalidSymbol(Token identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => $"{actualKind} '{identifierToken.Text}' is not a '{expectedKind}'";

    public static string InvalidBreakOrContinue() => "No enclosing loop out of which to break or continue";

    public static string InvalidReturn() => "No enclosing function out of which to return";

    public static string InvalidReturnExpression(string functionName) => $"Since '{functionName}' returns void, a return keyword must not be followed by an expression";

    public static string InvalidReturnExpression(string functionName, PrimType expectedType) => $"Function '{functionName}' expects an expression of a type convertible to '{expectedType}'";

    public static string NotAllPathsReturn() => "Not all code paths return a value";

    public static string UnreachableCode() => "Unreachable code detected";
}
