using CodeAnalysis.Binding;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    [Fact]
    public void Bind_AddExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 + 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.AddExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_SubtractExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 - 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.SubtractExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_MultiplyExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 * 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.MultiplyExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_DivideExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 / 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.DivideExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_ModuloExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 % 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.ModuloExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_PowerExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 ** 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.PowerExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_LeftShiftExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 << 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.LeftShiftExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_RightShiftExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 >> 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.RightShiftExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_LogicalOrExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("true || false"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.LogicalOrExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_LogicalAndExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("true && true"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.LogicalAndExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_BitwiseOrExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 | 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.BitwiseOrExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_BitwiseAndExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 & 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.BitwiseAndExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_ExclusiveOrExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 ^ 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.ExclusiveOrExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_EqualsExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 == 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.EqualsExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_NotEqualsExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 != 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.NotEqualsExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_LessThanExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 < 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.LessThanExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_LessThanOrEqualExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 <= 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.LessThanOrEqualExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_GreaterThanExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 > 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.GreaterThanExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_GreaterThanOrEqualExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("2 >= 2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.GreaterThanOrEqualExpression, node.BoundKind);
    }
    [Fact]
    public void Bind_CoalesceExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("""
            x: ?i32 = null;
            x ?? 2
            """));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.CoalesceExpression, node.BoundKind);
    }
}
