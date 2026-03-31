using System.Collections.Immutable;
using System.Diagnostics;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

internal static class BinderDeclareExtensions
{
    extension(GlobalSymbolBinder binder)
    {
        public void DeclareSymbols(ImmutableArray<SyntaxTree> syntaxTrees)
        {
            binder.DeclareTypes(syntaxTrees);
            binder.DeclareMembers(syntaxTrees);
        }

        private void DeclareTypes(ImmutableArray<SyntaxTree> syntaxTrees)
        {
            foreach (var compilationUnit in syntaxTrees.Select(x => x.CompilationUnit))
            {
                if (compilationUnit.Module is not null)
                {
                    var module = binder.DeclareModule(compilationUnit.Module);
                    binder = new GlobalSymbolBinder(module, binder);
                }

                foreach (var structSyntax in compilationUnit.Declarations.Select(x => x.Declaration).OfType<StructDeclarationSyntax>())
                {
                    binder.DeclareStruct(structSyntax);
                }
            }
        }

        private void DeclareMembers(ImmutableArray<SyntaxTree> syntaxTrees)
        {
            foreach (var compilationUnit in syntaxTrees.Select(x => x.CompilationUnit))
            {
                if (compilationUnit.Module is not null)
                {
                    if (!binder.TryLookup<ModuleSymbol>(compilationUnit.Module.Name.FullName, out var module))
                    {
                        throw new UnreachableException($"Missing {nameof(ModuleSymbol)} '{compilationUnit.Module.Name.FullName}'");
                    }

                    binder = new GlobalSymbolBinder(module, binder);
                }

                foreach (var declaration in compilationUnit.Declarations.Select(x => x.Declaration))
                {
                    switch (declaration)
                    {
                        case StructDeclarationSyntax structDeclaration:
                            binder.DeclareStructMembers(structDeclaration);
                            break;

                        case VariableDeclarationSyntax variable:
                            binder.DeclareVariable(variable);
                            break;
                    }
                }
            }
        }

        private ModuleSymbol DeclareModule(ModuleDeclarationSyntax syntax)
        {
            var module = new ModuleSymbol(syntax, syntax.Name.FullName, binder.Module);
            _ = binder.TryDeclare(module);
            return module;
        }

        private void DeclareStruct(StructDeclarationSyntax syntax)
        {
            var @struct = new StructTypeSymbol(syntax, syntax.Name.FullName, binder.Module);

            if (!binder.TryDeclare(@struct))
            {
                binder.ReportSymbolRedeclaration(syntax.SourceSpan, @struct.Name);
            }
        }

        private void DeclareStructMembers(StructDeclarationSyntax syntax)
        {
            if (!binder.TryLookup<StructTypeSymbol>(syntax.Name.FullName, out var structType))
            {
                // TODO: Should this be a diagnostics instead?
                throw new UnreachableException($"Missing {nameof(StructTypeSymbol)} '{syntax.Name.FullName}'");
            }

            foreach (var propertySyntax in syntax.Properties)
            {
                var propertyType = binder.BindType(propertySyntax.TypeClause.Type);
                var property = new PropertySymbol(propertySyntax, propertySyntax.Name.FullName, propertyType, structType, Modifiers.None);
                if (!structType.TryDeclare(property))
                {
                    binder.ReportSymbolRedeclaration(propertySyntax.SourceSpan, property.Name);
                }
            }
        }

        private void DeclareVariable(VariableDeclarationSyntax syntax)
        {
            var variableType = syntax.TypeClause is not null ? binder.BindType(syntax.TypeClause.Type) : binder.Module.Never;
            if (variableType == binder.Module.Never)
            {
                binder.ReportInvalidImplicitType(syntax.SourceSpan, variableType.FullName);
            }

            var modifiers = syntax.BindingKeyword.SyntaxKind is SyntaxKind.LetKeyword ? Modifiers.ReadOnly : Modifiers.None;

            var variable = new VariableSymbol(syntax, syntax.Name.FullName, variableType, binder.Module, modifiers);
            if (!binder.TryDeclare(variable))
            {
                binder.ReportSymbolRedeclaration(syntax.SourceSpan, variable.Name);
            }
        }
    }
}
