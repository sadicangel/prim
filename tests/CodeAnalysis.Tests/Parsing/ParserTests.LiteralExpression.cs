namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_TrueLiteralExpression()
    {
        var node = ParseExpression("true");
        Assert.Equal(SyntaxKind.TrueLiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_FalseLiteralExpression()
    {
        var node = ParseExpression("false");
        Assert.Equal(SyntaxKind.FalseLiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_NullLiteralExpression()
    {
        var node = ParseExpression("null");
        Assert.Equal(SyntaxKind.NullLiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_I8LiteralExpression()
    {
        var node = ParseExpression("42i8");
        Assert.Equal(SyntaxKind.I8LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_U8LiteralExpression()
    {
        var node = ParseExpression("42u8");
        Assert.Equal(SyntaxKind.U8LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_I16LiteralExpression()
    {
        var node = ParseExpression("42i16");
        Assert.Equal(SyntaxKind.I16LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_U16LiteralExpression()
    {
        var node = ParseExpression("42u16");
        Assert.Equal(SyntaxKind.U16LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_I32LiteralExpression()
    {
        var node = ParseExpression("42i32");
        Assert.Equal(SyntaxKind.I32LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_U32LiteralExpression()
    {
        var node = ParseExpression("42u32");
        Assert.Equal(SyntaxKind.U32LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_I64LiteralExpression()
    {
        var node = ParseExpression("42i64");
        Assert.Equal(SyntaxKind.I64LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_U64LiteralExpression()
    {
        var node = ParseExpression("42u64");
        Assert.Equal(SyntaxKind.U64LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_F16LiteralExpression()
    {
        var node = ParseExpression("4.2f16");
        Assert.Equal(SyntaxKind.F16LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_F32LiteralExpression()
    {
        var node = ParseExpression("4.2f32");
        Assert.Equal(SyntaxKind.F32LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_F64LiteralExpression()
    {
        var node = ParseExpression("4.2f64");
        Assert.Equal(SyntaxKind.F64LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_StrLiteralExpression()
    {
        var node = ParseExpression("\"str\"");
        Assert.Equal(SyntaxKind.StrLiteralExpression, node.SyntaxKind);
    }
}
