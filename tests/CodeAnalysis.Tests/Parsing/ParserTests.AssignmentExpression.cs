namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_AssignmentExpression_Simple()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a = 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.SimpleAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Add()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a += 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.AddAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Subtract()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a -= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.SubtractAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Multiply()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a *= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.MultiplyAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Divide()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a /= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.DivideAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Modulo()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a %= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.ModuloAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Power()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a **= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.PowerAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_And()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a &= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.AndAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_ExclusiveOr()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a ^= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.ExclusiveOrAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Or()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a |= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.OrAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_LeftShift()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a <<= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.LeftShiftAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_RightShift()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a >>= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.RightShiftAssignmentExpression, node.SyntaxKind);
    }
    [Fact]
    public void Parse_AssignmentExpression_Coalesce()
    {
        var unit = SyntaxTree.ParseScript(new SourceText("a ??= 3"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        Assert.Equal(SyntaxKind.CoalesceAssignmentExpression, node.SyntaxKind);
    }
}
