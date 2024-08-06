using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal readonly record struct QualifiedName(ModuleSymbol ContainingModule, string Name)
{
    public override string ToString()
    {
        if (ContainingModule.IsGlobal)
        {
            return Name;
        }
        else
        {
            return $"{ContainingModule.QualifiedName}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}{Name}";
        }
    }

    public static implicit operator string(QualifiedName name) => name.ToString();
}
