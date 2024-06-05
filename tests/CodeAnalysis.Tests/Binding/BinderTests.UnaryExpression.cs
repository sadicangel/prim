using CodeAnalysis.Binding;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    [Fact]
    public void Bind_NotExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("!true"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.NotExpression, node.BoundKind);
    }

    [Fact]
    public void Bind_UnaryMinusExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("-2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.UnaryMinusExpression, node.BoundKind);
    }

    [Fact]
    public void Bind_UnaryPlusExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("+2"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.UnaryPlusExpression, node.BoundKind);
    }

    [Fact]
    public void Bind_OnesComplementExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("~1"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.OnesComplementExpression, node.BoundKind);
    }

    [Fact]
    public void Bind_PrefixIncrementExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("a: i32 = 0; ++a"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.PrefixIncrementExpression, node.BoundKind);
    }

    [Fact]
    public void Bind_PrefixDecrementExpression()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("a: i32 = 0; --a"));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.Root.BoundNodes[^1];
        Assert.Equal(BoundKind.PrefixDecrementExpression, node.BoundKind);
    }

}
