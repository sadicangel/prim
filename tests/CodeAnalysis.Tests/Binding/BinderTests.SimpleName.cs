using CodeAnalysis.Binding;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    [Fact]
    public void Bind_SimpleName()
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText("""
            a: i32 = 2;
            a
            """));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.CompilationUnit.BoundNodes[^1];
        Assert.Equal(BoundKind.LocalReference, node.BoundKind);
    }
}
