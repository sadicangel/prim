namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_TrueLiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("true"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.TrueLiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_FalseLiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("false"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.FalseLiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_NullLiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("null"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.NullLiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_I8LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("42i8"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.I8LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_U8LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("42u8"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.U8LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_I16LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("42i16"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.I16LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_U16LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("42u16"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.U16LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_I32LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("42i32"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.I32LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_U32LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("42u32"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.U32LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_I64LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("42i64"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.I64LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_U64LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("42u64"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.U64LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_F16LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("4.2f16"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.F16LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_F32LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("4.2f32"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.F32LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_F64LiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("4.2f64"));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.F64LiteralExpression, node.SyntaxKind);
    }

    [Fact]
    public void Parse_StrLiteralExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("\"str\""));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.StrLiteralExpression, node.SyntaxKind);
    }
}
