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
    public static string AmbiguousBinaryOperator(Token @operator, PrimType leftType, PrimType rightType) => $"Binary operator '{@operator.Text}' is ambiguous on operands of type '{leftType.Name}' and '{rightType.Name}'";
    public static string InvalidArgumentType(Parameter parameter, PrimType actualType) => $"Invalid expression of type '{actualType.Name}' provided for parameter '{parameter.Name}' of type '{parameter.Type.Name}'";
    public static string InvalidExpressionType(PrimType expectedType, PrimType actualType) => $"Invalid expression of type '{actualType.Name}'. Expected '{expectedType.Name}'";
    public static string InvalidNumberOfArguments(FunctionType functionType, int actualNumberOfArguments) => $"Function '{functionType.Name}' requires {functionType.Parameters.Count} arguments but was given {actualNumberOfArguments}";
    public static string InvalidSymbolType(PrimType expectedType, PrimType actualType) => $"Invalid symbol of type '{actualType.Name}'. Expected '{expectedType.Name}'";
    public static string InvalidTypeConversion(PrimType sourceType, PrimType targetType) => $"Invalid conversion from type '{sourceType.Name}' to '{targetType.Name}'";
    public static string SymbolReassignment(Symbol symbol) => $"Reassignment of read-only symbol '{symbol.Name}'";
    public static string SymbolRedeclaration(Symbol symbol) => $"Redeclaration of symbol '{symbol.Name}'";
    public static string UndefinedBinaryOperator(Token @operator, PrimType leftType, PrimType rightType) => $"Binary operator '{@operator.Text}' is not defined for types '{leftType.Name}' and '{rightType.Name}'";
    public static string UndefinedType(PrimType type) => $"Undefined type '{type.Name}'";
    public static string ReportUndefinedSymbol(Symbol symbol) => $"Undefined symbol '{symbol.Name}'";
    public static string UndefinedUnaryOperator(Token @operator, PrimType operandType) => $"Unary operator '{@operator.Text}' is not defined for type '{operandType.Name}'";



    public static string InvalidImplicitConversion(PrimType sourceType, PrimType destinationType) => $"Invalid implicit conversion from type '{sourceType}' to '{destinationType}'. An explicit conversion exists (are you missing a cast?)";

    public static string RedundantConversion() => "Conversion is redundant";



    public static string InvalidExpressionType(PrimType actualType) => $"Invalid expression of type '{actualType}'";

    public static string InvalidSymbol(Token identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => $"{actualKind} '{identifierToken.Text}' is not a '{expectedKind}'";

    public static string InvalidBreakOrContinue() => "No enclosing loop out of which to break or continue";

    public static string InvalidReturn() => "No enclosing function out of which to return";

    public static string InvalidReturnExpression(string functionName) => $"Since '{functionName}' returns void, a return keyword must not be followed by an expression";

    public static string InvalidReturnExpression(string functionName, PrimType expectedType) => $"Function '{functionName}' expects an expression of a type convertible to '{expectedType}'";

    public static string NotAllPathsReturn() => "Not all code paths return a value";

    public static string UnreachableCode() => "Unreachable code detected";
}

file static class Extensions
{
    public static string MergeText(this IReadOnlyList<Token> tokens)
    {
        var length = tokens.Sum(t => t.Text.Length);
        return string.Create(length, tokens, static (span, list) =>
        {
            int i = 0;
            foreach (var token in list)
                foreach (var @char in token.Text)
                    span[i++] = @char;

        });
    }
}
