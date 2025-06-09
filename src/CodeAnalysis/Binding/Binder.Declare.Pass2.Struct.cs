using System.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

partial class Binder
{
    public static void DeclarePass2Struct(StructDeclarationSyntax syntax, BindingContext context)
    {
        if (!context.TryLookup<StructSymbol>(syntax.Name.FullName, out var @struct))
        {
            throw new UnreachableException($"Missing {nameof(StructSymbol)} '{syntax.Name.FullName}'");
        }

        foreach (var propertySyntax in syntax.Properties)
        {
            var propertyType = BindType(propertySyntax.TypeClause.Type, context);
            var property = new PropertySymbol(propertySyntax, propertySyntax.Name.FullName, propertyType, @struct, Modifiers.None);
            if (!@struct.TryDeclare(property))
            {
                context.Diagnostics.ReportSymbolRedeclaration(propertySyntax.SourceSpan, property.Name);
            }
        }
    }
}
