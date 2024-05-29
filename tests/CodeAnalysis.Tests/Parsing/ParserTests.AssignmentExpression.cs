namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_AssignmentExpression_Simple()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a = 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.SimpleAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Add()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a += 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.AddAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Subtract()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a -= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.SubtractAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Multiply()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a *= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.MultiplyAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Divide()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a /= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.DivideAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Modulo()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a %= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.ModuloAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Power()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a **= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.PowerAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_And()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a &= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.AndAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_ExclusiveOr()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a ^= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.ExclusiveOrAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Or()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a |= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.OrAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_LeftShift()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a <<= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.LeftShiftAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_RightShift()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a >>= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.RightShiftAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Coalesce()
    {
        var tree = SyntaxTree.ParseScript(new SourceText("a ??= 3"));
        var node = Assert.Single(tree.Root.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        Assert.Equal(SyntaxKind.CoalesceAssignmentExpression, node.SyntaxKind);
    }
}
