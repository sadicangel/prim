using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal static class BinderDeclareExtensions
{
    extension(GlobalSymbolBinder binder)
    {
        public void DeclareSymbols(ImmutableArray<SyntaxTree> syntaxTrees)
        {
            var nonModuleDeclarationNames = GetNonModuleDeclarationNames(syntaxTrees);
            binder.DeclareTypes(syntaxTrees, nonModuleDeclarationNames);
            binder.DeclareMembers(syntaxTrees);
        }

        private void DeclareTypes(ImmutableArray<SyntaxTree> syntaxTrees, HashSet<string> nonModuleDeclarationNames)
        {
            foreach (var declaration in syntaxTrees.SelectMany(x => x.CompilationUnit.Declarations))
            {
                switch (declaration.Initializer)
                {
                    case ModuleExpressionSyntax:
                        binder.DeclareModule(declaration, nonModuleDeclarationNames);
                        break;

                    case TypeExpressionSyntax:
                        binder.DeclareStruct(declaration);
                        break;
                }
            }
        }

        private void DeclareMembers(ImmutableArray<SyntaxTree> syntaxTrees)
        {
            foreach (var declaration in syntaxTrees.SelectMany(x => x.CompilationUnit.Declarations))
            {
                switch (declaration.Initializer)
                {
                    case TypeExpressionSyntax typeExpression:
                        binder.DeclareStructMembers(declaration, typeExpression);
                        break;

                    case ModuleExpressionSyntax:
                        break;

                    default:
                        binder.DeclareVariable(declaration);
                        break;
                }
            }
        }

        private ModuleSymbol? DeclareModule(GlobalDeclarationSyntax syntax, HashSet<string> nonModuleDeclarationNames)
        {
            var module = binder.Module;
            var parts = syntax.Name.Name.ToArray();
            var pathParts = new List<string>(parts.Length);

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                pathParts.Add(part);
                var path = string.Join(SyntaxFacts.NameSeparator.ToString(), pathParts);
                var isFinalPart = i == parts.Length - 1;

                if (!isFinalPart && nonModuleDeclarationNames.Contains(path))
                {
                    binder.ReportInvalidModulePath(syntax.Name.SourceSpan, path);
                    return null;
                }

                if (module.TryLookup<Symbol>(part, out var existing))
                {
                    if (existing is ModuleSymbol existingModule)
                    {
                        module = existingModule;
                        continue;
                    }

                    if (isFinalPart)
                        binder.ReportSymbolRedeclaration(syntax.Name.SourceSpan, part);
                    else
                        binder.ReportInvalidModulePath(syntax.Name.SourceSpan, path);

                    return null;
                }

                var newModule = new ModuleSymbol(syntax, part, module);
                if (!module.TryDeclare(newModule))
                {
                    binder.ReportSymbolRedeclaration(syntax.Name.SourceSpan, part);
                    return null;
                }

                module = newModule;
            }

            return module;
        }

        private void DeclareStruct(GlobalDeclarationSyntax syntax)
        {
            if (syntax.Name is QualifiedNameSyntax)
            {
                binder.ReportInvalidQualifiedDeclarationName(syntax.Name.SourceSpan);
                return;
            }

            var @struct = new StructTypeSymbol(syntax, syntax.Name.FullName, binder.Module);
            if (!binder.TryDeclare(@struct))
                binder.ReportSymbolRedeclaration(syntax.SourceSpan, @struct.Name);
        }

        private void DeclareStructMembers(GlobalDeclarationSyntax syntax, TypeExpressionSyntax typeExpression)
        {
            if (syntax.Name is QualifiedNameSyntax)
                return;

            if (!binder.TryLookup<StructTypeSymbol>(syntax.Name.FullName, out var structType))
            {
                binder.ReportUndefinedType(syntax.Name.SourceSpan, syntax.Name.FullName);
                return;
            }

            var typeBinder = new TypeBinder(structType, binder);
            foreach (var propertySyntax in typeExpression.Properties)
            {
                var propertyType = propertySyntax.Type is not null
                    ? binder.BindType(propertySyntax.Type)
                    : binder.Module.Unknown;
                var property = new PropertySymbol(
                    propertySyntax,
                    propertySyntax.Name.Name.Name,
                    propertyType,
                    structType,
                    propertySyntax.IsReadOnly ? Modifiers.ReadOnly : Modifiers.None);

                if (!typeBinder.TryDeclare(property))
                    binder.ReportSymbolRedeclaration(propertySyntax.SourceSpan, property.Name);
            }
        }

        private void DeclareVariable(GlobalDeclarationSyntax syntax)
        {
            if (syntax.Name is QualifiedNameSyntax)
            {
                binder.ReportInvalidQualifiedDeclarationName(syntax.Name.SourceSpan);
                return;
            }

            var variableType = syntax.Type is not null
                ? binder.BindType(syntax.Type)
                : binder.Module.Unknown;
            var modifiers = syntax.IsReadOnly ? Modifiers.ReadOnly : Modifiers.None;
            var variable = new VariableSymbol(syntax, syntax.Name.FullName, variableType, binder.Module, modifiers);

            if (!binder.TryDeclare(variable))
                binder.ReportSymbolRedeclaration(syntax.SourceSpan, variable.Name);
        }

        private static HashSet<string> GetNonModuleDeclarationNames(ImmutableArray<SyntaxTree> syntaxTrees)
        {
            return syntaxTrees
                .SelectMany(x => x.CompilationUnit.Declarations)
                .Where(declaration => declaration.Initializer is not ModuleExpressionSyntax)
                .Select(declaration => declaration.Name.FullName)
                .ToHashSet(StringComparer.Ordinal);
        }
    }
}
