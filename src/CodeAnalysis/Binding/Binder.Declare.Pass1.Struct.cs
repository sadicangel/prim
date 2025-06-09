using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

partial class Binder
{
    public static void DeclarePass1Struct(StructDeclarationSyntax syntax, BindingContext context)
    {
        var @struct = new StructSymbol(syntax, syntax.Name.FullName, context.Module);

        if (!context.TryDeclare(@struct))
        {
            context.Diagnostics.ReportSymbolRedeclaration(syntax.SourceSpan, @struct.Name);
        }
    }
}
