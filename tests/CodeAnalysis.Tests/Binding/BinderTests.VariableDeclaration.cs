using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    [Fact]
    public void Bind_VariableDeclaration()
    {
        var scope = BoundScope.Global();
        _ = BoundTree.BindSymbols(SyntaxTree.Parse(new SourceText($"a: i32 = 2;")), scope);
        _ = Assert.IsType<VariableSymbol>(scope.Lookup("a"));
    }
}
