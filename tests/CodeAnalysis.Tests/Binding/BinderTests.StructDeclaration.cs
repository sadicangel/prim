using CodeAnalysis.Binding;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    [Fact]
    public void Bind_StructDeclaration()
    {
        var syntaxTree = SyntaxTree.Parse(new SourceText("""
            Point2d: struct = {
                x: i32;
                y: i32;
            }
            """));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.CompilationUnit.BoundNodes[^1];
        Assert.Equal(BoundKind.StructDeclaration, node.BoundKind);
    }
}
