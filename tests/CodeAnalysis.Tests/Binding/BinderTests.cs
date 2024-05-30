using CodeAnalysis.Binding;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    private static readonly BoundScope s_globalScope = BoundScope.Global();
    private readonly BoundScope _scope = BoundScope.ChildOf(s_globalScope);
}
