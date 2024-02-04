using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using System.Diagnostics;

namespace CodeAnalysis.Tests.Syntax;
public sealed class SyntaxTree_Parse_should
{
    [Fact]
    public void Parse_NameExpression()
    {
        var tree = SyntaxTree.ParseScript("""
            a
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<NameExpression>(node);
        Assert.Equal("a", expr.Identifier.Text);
    }

    [Fact]
    public void Parse_AssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript("""
            a = 2
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<AssignmentExpression>(node);
        Assert.Equal("a", expr.Identifier.Text);
        Assert.Equal("=", expr.Equal.Text);
        Assert.Equal("2", expr.Expression.Text);
    }

    [Fact]
    public void Parse_AssignmentExpression_Block()
    {
        var tree = SyntaxTree.ParseScript("""
            a = {}
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<AssignmentExpression>(node);
        Assert.Equal("a", expr.Identifier.Text);
        Assert.Equal("=", expr.Equal.Text);
        var block = Assert.IsType<BlockExpression>(expr.Expression);
        Assert.Equal("{}", block.Text);
    }

    [Fact]
    public void Parse_AssignmentExpression_IfElse()
    {
        var tree = SyntaxTree.ParseScript("""
            a = if (a > b) 1 else 2;
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<AssignmentExpression>(node);
        Assert.Equal("a", expr.Identifier.Text);
        Assert.Equal("=", expr.Equal.Text);
        var ifElse = Assert.IsType<IfElseExpression>(expr.Expression);
        Assert.Equal("if (a > b) 1 else 2;", ifElse.Text);
    }

    public static TheoryData<string> CompoundOperatorsData() => new()
    {
        TokenKind.PlusEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.PlusEqual)}"),
        TokenKind.MinusEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.MinusEqual)}"),
        TokenKind.StarEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.StarEqual)}"),
        TokenKind.SlashEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.SlashEqual)}"),
        TokenKind.PercentEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.PercentEqual)}"),
        TokenKind.StarStarEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.StarStarEqual)}"),
        TokenKind.LessLessEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.LessLessEqual)}"),
        TokenKind.GreaterGreaterEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.GreaterGreaterEqual)}"),
        TokenKind.AmpersandEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.AmpersandEqual)}"),
        TokenKind.PipeEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.PipeEqual)}"),
        TokenKind.HatEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.HatEqual)}"),
        TokenKind.HookHookEqual.GetText() ?? throw new UnreachableException($"Missing text for {nameof(TokenKind.HookHookEqual)}"),
    };

    [Theory]
    [MemberData(nameof(CompoundOperatorsData))]
    public void Parse_AssignmentExpression_Compound(string @operator)
    {
        var tree = SyntaxTree.ParseScript($"""
            a {@operator} 2
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<AssignmentExpression>(node);
        Assert.Equal("a", expr.Identifier.Text);
        Assert.Equal(@operator, expr.Equal.Text);
        Assert.Equal("2", expr.Expression.Text);
    }

    [Fact]
    public void Parse_DeclarationExpression_of_immutable_var()
    {
        var tree = SyntaxTree.ParseScript("""
            a: i32 = 2
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<DeclarationExpression>(node);
        Assert.Equal("a", expr.Identifier.Text);
        Assert.Equal(":", expr.Colon.Text);
        Assert.Null(expr.Mutable);
        Assert.Equal("i32", expr.TypeNode.Text);
        Assert.Equal("=", expr.Equal.Text);
        Assert.Equal("2", expr.Expression.Text);
    }

    [Fact]
    public void Parse_DeclarationExpression_of_mutable_var()
    {
        var tree = SyntaxTree.ParseScript("""
            a: mutable i32 = 2
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<DeclarationExpression>(node);
        Assert.Equal("a", expr.Identifier.Text);
        Assert.Equal(":", expr.Colon.Text);
        Assert.Equal("mutable", expr.Mutable!.Text);
        Assert.Equal("i32", expr.TypeNode.Text);
        Assert.Equal("=", expr.Equal.Text);
        Assert.Equal("2", expr.Expression.Text);
    }

    public static TheoryData<string[]> CallExpressionData() => new()
    {
        { [] },
        { ["b"] },
        { ["b", "c + 2"] },
        { ["b", "c + 2", "d[2]"] },
    };
    [Theory]
    [MemberData(nameof(CallExpressionData))]
    public void Parse_CallExpression(string[] arguments)
    {
        var tree = SyntaxTree.ParseScript($"""
            a({string.Join(", ", arguments)})
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<BinaryExpression>(node);
        Assert.Equal("a", expr.Left.Text);
        Assert.Equal("(", expr.Operator.Text);
        var args = Assert.IsType<ArgumentListExpression>(expr.Right);
        Assert.Equal(arguments.Length, args.Count);
        Assert.All(args, arg => Assert.IsAssignableFrom<Expression>(arg));
        Assert.NotNull(expr.OperatorClose);
        Assert.Equal(")", expr.OperatorClose.Text);
    }

    [Theory]
    [MemberData(nameof(LiteralData))]
    public void Parse_LiteralExpression(string text, object? value)
    {
        var tree = SyntaxTree.ParseScript(text);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsAssignableFrom<LiteralExpression>(node);
        Assert.Equal(text, expr.Literal.Text);
        Assert.Equal(value, expr.Literal.Value);
    }

    [Fact]
    public void Parse_GroupExpression()
    {
        var tree = SyntaxTree.ParseScript("""
            (a)
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<GroupExpression>(node);
        Assert.Equal("(", expr.ParenthesisOpen.Text);
        Assert.IsAssignableFrom<Expression>(expr.Expression);
        Assert.Equal(")", expr.ParenthesisClose.Text);
    }

    public static TheoryData<string> UnaryOperatorsData()
    {
        var data = new TheoryData<string>();
        data.AddRange([.. Enum.GetValues<TokenKind>().Where(k => k.IsUnaryOperator()).Select(k => k.GetText())]);
        return data;
    }
    [Theory]
    [MemberData(nameof(UnaryOperatorsData))]
    public void Parse_UnaryExpression(string @operator)
    {
        var tree = SyntaxTree.ParseScript($"""
            {@operator}a
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<UnaryExpression>(node);
        Assert.Equal(@operator, expr.Operator.Text);
        Assert.IsAssignableFrom<Expression>(expr.Operand);
    }

    public static TheoryData<string> BinaryOperatorsData()
    {
        var data = new TheoryData<string>();
        data.AddRange([.. Enum.GetValues<TokenKind>().Where(k => k.IsBinaryOperator()).Select(k => k.GetText())]);
        return data;
    }
    [Theory]
    [MemberData(nameof(BinaryOperatorsData))]
    public void Parse_BinaryExpression(string @operator)
    {
        var tree = SyntaxTree.ParseScript($"""
            a {@operator} b
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsAssignableFrom<BinaryExpression>(node);
        Assert.IsAssignableFrom<Expression>(expr.Left);
        Assert.Equal(@operator, expr.Operator.Text);
        Assert.IsAssignableFrom<Expression>(expr.Right);
    }

    public static TheoryData<string, object?> LiteralData() => new()
    {
        { "false", false },
        { "true", true },
        { "1234", 1234 },
        { "12.3", 12.3f },
        { "\"string\"", "string" },
        { "null", null }
    };

    [Fact]
    public void Parse_ReturnExpression_without_result()
    {
        var tree = SyntaxTree.ParseScript("""
            return;
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<ReturnExpression>(node);
        Assert.Equal("return", expr.Return.Text);
        Assert.Equal(";", expr.Result.Text);
    }

    [Fact]
    public void Parse_ReturnExpression_with_result()
    {
        var tree = SyntaxTree.ParseScript("""
            return 0;
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<ReturnExpression>(node);
        Assert.Equal("return", expr.Return.Text);
        Assert.Equal("0;", expr.Result.Text);
    }

    [Fact]
    public void Parse_ContinueExpression_without_result()
    {
        var tree = SyntaxTree.ParseScript("""
            continue;
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<ContinueExpression>(node);
        Assert.Equal("continue", expr.Continue.Text);
        Assert.Equal(";", expr.Result.Text);
    }

    [Fact]
    public void Parse_ContinueExpression_with_result()
    {
        var tree = SyntaxTree.ParseScript("""
            continue 0;
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<ContinueExpression>(node);
        Assert.Equal("continue", expr.Continue.Text);
        Assert.Equal("0;", expr.Result.Text);
    }

    [Fact]
    public void Parse_BreakExpression_without_result()
    {
        var tree = SyntaxTree.ParseScript("""
            break;
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<BreakExpression>(node);
        Assert.Equal("break", expr.Break.Text);
        Assert.Equal(";", expr.Result.Text);
    }

    [Fact]
    public void Parse_BreakExpression_with_result()
    {
        var tree = SyntaxTree.ParseScript("""
            break 0;
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<BreakExpression>(node);
        Assert.Equal("break", expr.Break.Text);
        Assert.Equal("0;", expr.Result.Text);
    }

    [Fact]
    public void Parse_WhileExpression()
    {
        var tree = SyntaxTree.ParseScript("""
            while (true) {
                a: i32 = 2;
                b: i32 = 3;
                c: i32 = a + b;
                return c;
            }
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<WhileExpression>(node);
        Assert.Equal("while", expr.While.Text);
        Assert.Equal("(", expr.ParenthesisOpen.Text);
        Assert.IsAssignableFrom<Expression>(expr.Condition);
        Assert.Equal(")", expr.ParenthesisClose.Text);
        Assert.IsAssignableFrom<Expression>(expr.Body);
    }

    [Fact]
    public void Parse_ForExpression()
    {
        var tree = SyntaxTree.ParseScript("""
            for (a : get_range()) {
                print(a);
            }
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<ForExpression>(node);
        Assert.Equal("for", expr.For.Text);
        Assert.Equal("(", expr.ParenthesisOpen.Text);
        Assert.Equal("a", expr.Identifier.Text);
        Assert.Equal(":", expr.Colon.Text);
        Assert.Equal(")", expr.ParenthesisClose.Text);
        Assert.IsAssignableFrom<Expression>(expr.Body);
    }

    [Fact]
    public void Parse_IfElseExpression()
    {
        var tree = SyntaxTree.ParseScript("""
            if (a > b) { 2 } else 3;
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<IfElseExpression>(node);
        Assert.Equal("if", expr.If.Text);
        Assert.Equal("(", expr.ParenthesisOpen.Text);
        Assert.IsAssignableFrom<Expression>(expr.Condition);
        Assert.Equal(")", expr.ParenthesisClose.Text);
        Assert.IsType<BlockExpression>(expr.Then);
        Assert.Equal("else", expr.ElseToken.Text);
        Assert.IsType<InlineExpression>(expr.Else);
    }

    [Fact]
    public void Parse_BlockExpression()
    {
        var tree = SyntaxTree.ParseScript("""
            {
                a: i32 = 2;
                b: i32 = 5;
                if (a * b > 0) print(a + b) else print("error");
            }
            """);
        var node = Assert.Single(tree.Root.Nodes);
        var expr = Assert.IsType<BlockExpression>(node);
        Assert.Equal("{", expr.BraceOpen.Text);
        Assert.All(expr.Expressions, expr => Assert.IsAssignableFrom<Expression>(expr));
        Assert.Equal("}", expr.BraceClose.Text);
    }
}
