using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using System.CodeDom.Compiler;

namespace CodeAnalysis.Binding;
internal static class BoundNodeWriterExtensions
{
    public static void WriteTo(this BoundNode node, TextWriter writer)
    {
        if (writer is not IndentedTextWriter indentedWriter)
            indentedWriter = new IndentedTextWriter(writer);
        WriteTo(node, indentedWriter);
    }

    public static void WriteTo(this BoundNode node, IndentedTextWriter writer) => writer.WriteNode(node);

    private static void WriteNode(this IndentedTextWriter writer, BoundNode node)
    {
        switch (node.NodeKind)
        {
            case BoundNodeKind.LiteralExpression:
                writer.WriteNode((BoundLiteralExpression)node);
                break;
            case BoundNodeKind.UnaryExpression:
                writer.WriteNode((BoundUnaryExpression)node);
                break;
            case BoundNodeKind.BinaryExpression:
                writer.WriteNode((BoundBinaryExpression)node);
                break;
            case BoundNodeKind.SymbolExpression:
                writer.WriteNode((BoundSymbolExpression)node);
                break;
            case BoundNodeKind.AssignmentExpression:
                writer.WriteNode((BoundAssignmentExpression)node);
                break;
            case BoundNodeKind.CallExpression:
                writer.WriteNode((BoundCallExpression)node);
                break;
            case BoundNodeKind.NeverExpression:
                writer.WriteNode((BoundNeverExpression)node);
                break;
            case BoundNodeKind.IfExpression:
                writer.WriteNode((BoundIfExpression)node);
                break;
            case BoundNodeKind.BlockStatement:
                writer.WriteNode((BoundBlockStatement)node);
                break;
            case BoundNodeKind.VariableDeclaration:
                writer.WriteNode((BoundVariableDeclaration)node);
                break;
            case BoundNodeKind.ExpressionStatement:
                writer.WriteNode((BoundExpressionStatement)node);
                break;
            case BoundNodeKind.IfStatement:
                writer.WriteNode((BoundIfStatement)node);
                break;
            case BoundNodeKind.WhileStatement:
                writer.WriteNode((BoundWhileStatement)node);
                break;
            case BoundNodeKind.ForStatement:
                writer.WriteNode((BoundForStatement)node);
                break;
            case BoundNodeKind.ConvertExpression:
                writer.WriteNode((BoundConvertExpression)node);
                break;
            case BoundNodeKind.FunctionDeclaration:
                writer.WriteNode((BoundFunctionDeclaration)node);
                break;
            case BoundNodeKind.GotoStatement:
                writer.WriteNode((BoundGotoStatement)node);
                break;
            case BoundNodeKind.ConditionalGotoStatement:
                writer.WriteNode((BoundConditionalGotoStatement)node);
                break;
            case BoundNodeKind.LabelStatement:
                writer.WriteNode((BoundLabelDeclaration)node);
                break;
            default:
                throw new NotSupportedException(node?.GetType()?.Name);
        }
    }
    /*
            var color = token.TokenKind switch
            {
                TokenKind k when k.IsNumber() => ConsoleColor.Cyan,
                TokenKind.String => ConsoleColor.Magenta,
                TokenKind.Identifier => ConsoleColor.DarkYellow,
                TokenKind k when k.IsKeyword() => ConsoleColor.Blue,
                _ => ConsoleColor.DarkGray,
            };
     */

    private static void WriteColored(this IndentedTextWriter writer, ConsoleColor color, ReadOnlySpan<char> text)
    {
        if (writer.InnerWriter == Console.Out)
            Console.ForegroundColor = color;

        writer.Write(text);

        if (writer.InnerWriter == Console.Out)
            Console.ResetColor();
    }

    private static void WritePunctuation(this IndentedTextWriter writer, TokenKind token)
    {
        writer.WriteColored(ConsoleColor.DarkGray, token.GetText() ?? throw new InvalidOperationException($"Token {token} has no text"));
    }

    private static void WriteKeyword(this IndentedTextWriter writer, TokenKind token)
    {
        writer.WriteKeyword(token.GetText() ?? throw new InvalidOperationException($"Token {token} has no text"));
    }

    private static void WriteKeyword(this IndentedTextWriter writer, ReadOnlySpan<char> keyword)
    {
        var color = keyword switch
        {
            "if" or "while" or "for" or "goto" or "break" or "continue" => ConsoleColor.DarkMagenta,
            _ => ConsoleColor.DarkBlue,
        };
        writer.WriteColored(color, keyword);
    }

    private static void WriteSymbol(this IndentedTextWriter writer, Symbol symbol)
    {
        var color = symbol.SymbolKind switch
        {
            SymbolKind.Type => ConsoleColor.DarkGreen,
            SymbolKind.Variable => ConsoleColor.Blue,
            SymbolKind.Function => ConsoleColor.DarkYellow,
            SymbolKind.Parameter => ConsoleColor.DarkGray,
            SymbolKind.Label => ConsoleColor.DarkGray,
            _ => throw new InvalidOperationException($"Unexpected symbol {symbol}"),
        };
        writer.WriteColored(color, symbol.Name);
    }

    private static void WriteNumber(this IndentedTextWriter writer, object? value)
    {
        writer.WriteColored(ConsoleColor.Cyan, value?.ToString() ?? "undefined");
    }

    private static void WriteString(this IndentedTextWriter writer, object? value)
    {
        writer.WriteColored(ConsoleColor.Magenta, value?.ToString() ?? "undefined");
    }

    private static void WriteNode(this IndentedTextWriter writer, BoundBlockStatement node)
    {
        writer.WritePunctuation(TokenKind.OpenBrace);
        writer.WriteLine();
        writer.Indent++;
        foreach (var statement in node.Statements)
            writer.WriteNode(statement);
        writer.Indent--;
        writer.WritePunctuation(TokenKind.CloseBrace);
        writer.WriteLine();
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundVariableDeclaration node)
    {
        writer.WriteKeyword(node.Variable.IsReadOnly ? TokenKind.Let : TokenKind.Var);
        writer.Write(" ");
        writer.WriteSymbol(node.Variable);
        writer.WritePunctuation(TokenKind.Colon);
        writer.Write(" ");
        writer.WriteSymbol(node.Variable.Type);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.Equal);
        writer.Write(" ");
        writer.WriteNode(node.Expression);
        writer.WritePunctuation(TokenKind.Semicolon);
        writer.WriteLine();
    }
    private static void WriteNode(this IndentedTextWriter writer, ParameterSymbol parameter)
    {
        writer.WriteSymbol(parameter);
        writer.WritePunctuation(TokenKind.Colon);
        writer.Write(" ");
        writer.WriteSymbol(parameter.Type);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundFunctionDeclaration node)
    {
        writer.WriteKeyword(TokenKind.Let);
        writer.Write(" ");
        writer.WriteSymbol(node.Function);
        writer.WritePunctuation(TokenKind.Colon);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.OpenParenthesis);
        if (node.Function.Parameters.Count > 0)
        {
            writer.WriteNode(node.Function.Parameters[0]);
            foreach (var parameter in node.Function.Parameters.Skip(1))
            {
                writer.WritePunctuation(TokenKind.Comma);
                writer.Write(" ");
                writer.WriteNode(parameter);
            }
        }
        writer.WritePunctuation(TokenKind.CloseParenthesis);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.Arrow);
        writer.Write(" ");
        writer.WriteSymbol(node.Function.Type);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.Equal);
        writer.Write(" ");
        writer.WriteNestedStatement(node.Body);

    }
    private static void WriteNode(this IndentedTextWriter writer, BoundExpressionStatement node)
    {
        writer.WriteNode(node.Expression);
        writer.WritePunctuation(TokenKind.Semicolon);
        writer.WriteLine();
    }
    private static void WriteNestedStatement(this IndentedTextWriter writer, BoundStatement node)
    {
        var needsIndentation = node is not BoundBlockStatement;
        if (needsIndentation)
            writer.Indent++;
        writer.WriteNode(node);
        if (needsIndentation)
            writer.Indent--;
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundIfStatement node)
    {
        writer.WriteKeyword(TokenKind.If);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.OpenParenthesis);
        writer.WriteNode(node.Condition);
        writer.WritePunctuation(TokenKind.CloseParenthesis);
        writer.WriteLine();
        writer.WriteNestedStatement(node.Then);
        if (node.HasElseClause)
        {
            writer.WriteKeyword(TokenKind.Else);
            writer.WriteLine();
            writer.WriteNestedStatement(node.Else);
        }
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundWhileStatement node)
    {
        writer.WriteKeyword(TokenKind.While);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.OpenParenthesis);
        writer.WriteNode(node.Condition);
        writer.WritePunctuation(TokenKind.CloseParenthesis);
        writer.WriteLine();
        writer.WriteNestedStatement(node.Body);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundForStatement node)
    {
        writer.WriteKeyword(TokenKind.For);
        writer.Write(" ");
        writer.WriteKeyword(TokenKind.Var);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.OpenParenthesis);
        writer.WriteSymbol(node.Variable);
        writer.Write(" ");
        writer.WriteKeyword(TokenKind.In);
        writer.Write(" ");
        writer.WriteNode(node.LowerBound);
        writer.WritePunctuation(TokenKind.Range);
        writer.WriteNode(node.UpperBound);
        writer.WritePunctuation(TokenKind.CloseParenthesis);
        writer.WriteLine();
        writer.WriteNestedStatement(node.Body);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundGotoStatement node)
    {
        writer.WriteKeyword("goto");
        writer.Write(" ");
        writer.WriteSymbol(node.Label);
        writer.WritePunctuation(TokenKind.Semicolon);
        writer.WriteLine();
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundConditionalGotoStatement node)
    {
        writer.WriteKeyword("goto");
        writer.Write(" ");
        writer.WriteSymbol(node.Label);
        writer.Write(" ");
        writer.WriteKeyword($"{(node.JumpIfTrue ? "if" : "unless")}");
        writer.Write(" ");
        writer.WriteNode(node.Condition);
        writer.WritePunctuation(TokenKind.Semicolon);
        writer.WriteLine();
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundLabelDeclaration node)
    {
        var hasIndentation = writer.Indent > 0;
        if (hasIndentation)
            writer.Indent--;
        writer.WriteSymbol(node.Label);
        writer.Write(":");
        writer.WriteLine();
        if (hasIndentation)
            writer.Indent++;
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundNeverExpression node)
    {
        writer.WriteSymbol(BuiltinTypes.Never);
    }
    private static void WriteNestedExpression(this IndentedTextWriter writer, int parentPrecedence, BoundExpression node)
    {
        switch (node)
        {
            case BoundUnaryExpression expression:
                writer.WriteNestedExpression(parentPrecedence, expression.Operator.TokenKind.GetUnaryOperatorPrecedence(), expression);
                break;
            case BoundBinaryExpression expression:
                writer.WriteNestedExpression(parentPrecedence, expression.Operator.TokenKind.GetBinaryOperatorPrecedence(), expression);
                break;
            default:
                writer.WriteNode(node);
                break;
        }
    }
    private static void WriteNestedExpression(this IndentedTextWriter writer, int parentPrecedence, int currentPrecedence, BoundExpression node)
    {
        var needsParenthesis = parentPrecedence >= currentPrecedence;
        if (needsParenthesis)
            writer.WritePunctuation(TokenKind.OpenParenthesis);
        writer.WriteNode(node);
        if (needsParenthesis)
            writer.WritePunctuation(TokenKind.CloseParenthesis);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundUnaryExpression node)
    {
        writer.WritePunctuation(node.Operator.TokenKind);
        writer.WriteNestedExpression(node.Operator.TokenKind.GetUnaryOperatorPrecedence(), node.Operand);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundBinaryExpression node)
    {
        var precedence = node.Operator.TokenKind.GetBinaryOperatorPrecedence();
        writer.WriteNestedExpression(precedence, node.Left);
        writer.Write(" ");
        writer.WritePunctuation(node.Operator.TokenKind);
        writer.Write(" ");
        writer.WriteNestedExpression(precedence, node.Right);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundLiteralExpression node)
    {
        switch (node.Type)
        {
            case var _ when node.Type == BuiltinTypes.Bool:
                writer.WriteKeyword(node.Value is true ? TokenKind.True : TokenKind.False);
                break;

            case var _ when node.Type == BuiltinTypes.Str:
                writer.WriteString(node.Value);
                break;

            case var _ when node.Type.IsNumber():
                writer.WriteNumber(node.Value);
                break;

            default:
                writer.Write(node.Value);
                break;
        }
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundSymbolExpression node)
    {
        writer.WriteSymbol(node.Symbol);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundAssignmentExpression node)
    {
        writer.WriteSymbol(node.Variable);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.Equal);
        writer.Write(" ");
        writer.WriteNode(node.Expression);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundIfExpression node)
    {
        writer.WriteKeyword(TokenKind.If);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.OpenParenthesis);
        writer.WriteNode(node.Condition);
        writer.WritePunctuation(TokenKind.CloseParenthesis);
        writer.Write(" ");
        writer.WriteNode(node.Then);
        writer.WriteKeyword(TokenKind.Else);
        writer.Write(" ");
        writer.WriteNode(node.Else);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundCallExpression node)
    {
        writer.WriteSymbol(node.Function);
        writer.WritePunctuation(TokenKind.OpenParenthesis);
        if (node.Arguments.Count > 0)
        {
            writer.WriteNode(node.Arguments[0]);
            foreach (var argument in node.Arguments.Skip(1))
            {
                writer.WritePunctuation(TokenKind.Comma);
                writer.Write(" ");
                writer.WriteNode(argument);
            }
        }
        writer.WritePunctuation(TokenKind.CloseParenthesis);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundConvertExpression node)
    {
        writer.WriteNode(node.Expression);
        writer.Write(" ");
        writer.WriteKeyword(TokenKind.As);
        writer.Write(" ");
        writer.WriteSymbol(node.Type);
    }
}
