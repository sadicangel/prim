namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_SimpleAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a = 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.SimpleAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AddAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a += 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.AddAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_SubtractAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a -= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.SubtractAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_MultiplyAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a *= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.MultiplyAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_DivideAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a /= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.DivideAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_ModuloAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a %= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.ModuloAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_PowerAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a **= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.PowerAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AndAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a &= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.AndAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_ExclusiveOrAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a ^= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.ExclusiveOrAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_OrAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a |= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.OrAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_LeftShiftAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a <<= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.LeftShiftAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_RightShiftAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a >>= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.RightShiftAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_CoalesceAssignmentExpression()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a ??= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.CoalesceAssignmentExpression, node.SyntaxKind);
    }
}
