namespace CodeAnalysis.Syntax;

public abstract record class Diagnostic(bool IsError, string Message)
{
    public static Diagnostic InvalidCharacter(char character) => new Error($"Invalid character input: {character}");

    public static Diagnostic UnexpectedToken(TokenKind actual, TokenKind expected) => new Error($"Unexpected token <{actual}>. Expected <{expected}>");

    public static Diagnostic InvalidNumber(string text) => new Error($"The value {text} is not a valid number");
    public static Diagnostic InvalidUnaryOperator(string operatorText, Type operandType) => new Error($"Unary operator '{operatorText}' is not defined for type {operandType.Name}");
    public static Diagnostic InvalidBinaryOperator(string operatorText, Type leftType, Type rightType) => new Error($"Binary operator '{operatorText}' is not defined for types {leftType.Name} and {rightType.Name}");

    private sealed record class Error(string Message) : Diagnostic(IsError: true, Message);

    private sealed record class Warning(string Message) : Diagnostic(IsError: false, Message);
}
