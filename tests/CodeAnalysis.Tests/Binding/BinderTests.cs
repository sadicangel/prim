using CodeAnalysis.Binding;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    private readonly BoundScope _scope = new(GlobalBoundScope.Instance);
}
