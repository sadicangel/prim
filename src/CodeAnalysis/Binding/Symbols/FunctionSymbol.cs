using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Types;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class FunctionSymbol(
    SyntaxNode Syntax,
    string Name,
    FunctionType Type,
    BoundList<VariableSymbol> Parameters,
    bool IsReadOnly)
    : Symbol(BoundKind.FunctionSymbol, Syntax, Name, IsReadOnly)
{
    public override FunctionType Type { get; } = Type;
    public PrimType ReturnType { get => Type.ReturnType; }

    public static FunctionSymbol FromConversion(SyntaxNode syntax, Conversion conversion, ParameterSyntax? parameterSyntax = null)
    {
        var parameter = conversion.Type.Parameters[0];
        var parameterSymbol = new VariableSymbol(
            parameterSyntax as SyntaxNode ?? SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
            parameter.Name,
            parameter.Type,
            IsReadOnly: false);

        return new FunctionSymbol(
            syntax,
            conversion.Name,
            conversion.Type,
            [parameterSymbol],
            IsReadOnly: true);
    }
}
