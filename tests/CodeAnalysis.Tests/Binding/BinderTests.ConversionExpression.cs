using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Tests.Binding;
public partial class BinderTests
{
    private static readonly TypeSymbol[] s_numberTypes = [
        PredefinedSymbols.I8,
        PredefinedSymbols.I16,
        PredefinedSymbols.I32,
        PredefinedSymbols.I64,
        PredefinedSymbols.I128,
        PredefinedSymbols.ISize,
        PredefinedSymbols.U8,
        PredefinedSymbols.U16,
        PredefinedSymbols.U32,
        PredefinedSymbols.U64,
        PredefinedSymbols.U128,
        PredefinedSymbols.USize,
        PredefinedSymbols.F16,
        PredefinedSymbols.F32,
        PredefinedSymbols.F64,
        PredefinedSymbols.F80,
        PredefinedSymbols.F128,
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
        Assert.Equal(BoundKind.UnaryExpression, node.BoundKind);
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
