using CodeAnalysis.Binding;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    private static readonly BoundScope s_globalScope = new GlobalBoundScope();
    private readonly BoundScope _scope = new(s_globalScope);
}
