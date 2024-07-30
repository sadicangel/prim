using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    private static readonly TypeSymbol[] s_numberTypes = [
        Predefined.I8,
        Predefined.I16,
        Predefined.I32,
        Predefined.I64,
        Predefined.I128,
        Predefined.ISize,
        Predefined.U8,
        Predefined.U16,
        Predefined.U32,
        Predefined.U64,
        Predefined.U128,
        Predefined.USize,
        Predefined.F16,
        Predefined.F32,
        Predefined.F64,
        Predefined.F80,
        Predefined.F128,
    ];

    [Theory]
    [MemberData(nameof(GetConversions))]
    public void Bind_ConversionExpression(string sourceTypeName, string targetTypeName)
    {
        var syntaxTree = SyntaxTree.ParseScript(new SourceText($"""
            x: {sourceTypeName} : 3;
            x as {targetTypeName}
            """));
        var boundTree = BoundTree.Bind(syntaxTree, _scope);
        var node = boundTree.CompilationUnit.BoundNodes[^1];
        Assert.Equal(BoundKind.ConversionExpression, node.BoundKind);
    }

    public static TheoryData<string, string> GetConversions()
    {
        var data = new TheoryData<string, string>();
        foreach (var sourceType in s_numberTypes)
            foreach (var targetType in s_numberTypes)
                if (sourceType != targetType)
                    data.Add(sourceType.Name, targetType.Name);
        return data;
    }
}
