using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    private static readonly string[] s_numberTypes = [
        PredefinedTypes.I8,
        PredefinedTypes.I16,
        PredefinedTypes.I32,
        PredefinedTypes.I64,
        PredefinedTypes.U8,
        PredefinedTypes.U16,
        PredefinedTypes.U32,
        PredefinedTypes.U64,
        PredefinedTypes.F16,
        PredefinedTypes.F32,
        PredefinedTypes.F64,
    ];

    [Theory]
    [MemberData(nameof(GetConversions))]
    public void Bind_ConversionExpression(string sourceTypeName, string targetTypeName)
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText($"""
            x:= 3{sourceTypeName};
            x as {targetTypeName}
            """));
        Assert.Empty(syntaxTree.Diagnostics);
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        Assert.Empty(boundTree.Diagnostics);
        var node = boundTree.CompilationUnit.BoundNodes[^1];
        Assert.Equal(BoundKind.ConversionExpression, node.BoundKind);
    }

    public static TheoryData<string, string> GetConversions()
    {
        var data = new TheoryData<string, string>();
        foreach (var sourceType in s_numberTypes)
            foreach (var targetType in s_numberTypes)
                if (sourceType != targetType)
                    data.Add(sourceType, targetType);
        return data;
    }
}
