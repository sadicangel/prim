using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

partial class Binder
{
    public static void DeclarePass2Variable(VariableDeclarationSyntax syntax, BindingContext context, bool allowInference)
    {
        var variableType = syntax.TypeClause is null
            ? allowInference ? context.Module.Unknown : context.Module.Never // TODO: Report diagnostic for not being able to infer
            : BindType(syntax.TypeClause.Type, context);

        var modifiers = syntax.LetOrVarKeyword.SyntaxKind is SyntaxKind.LetKeyword ? Modifiers.ReadOnly : Modifiers.None;

        var variable = new VariableSymbol(syntax, syntax.Name.FullName, variableType, context.Module, modifiers);
        if (!context.TryDeclare(variable))
        {
            context.Diagnostics.ReportSymbolRedeclaration(syntax.SourceSpan, variable.Name);
        }
    }
}
