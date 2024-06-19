using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    [Fact]
    public void Bind_VariableDeclaration()
    {
        _ = BoundTree.Bind(SyntaxTree.Parse(new SourceText($"a: i32 = 2;")), _scope).CompilationUnit;
        _ = Assert.IsType<VariableSymbol>(_scope.Lookup("a"));
    }
}
