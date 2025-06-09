using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundVariableDeclaration BindVariableDeclaration(VariableDeclarationSyntax syntax, BindingContext context)
    {
        if (!context.TryLookup<VariableSymbol>(syntax.Name.FullName, out var variable))
            throw new UnreachableException($"Unexpected symbol for '{nameof(VariableDeclarationSyntax)}'");

        var expression = syntax.InitClause?.Expression is ExpressionSyntax initExpression
            ? BindExpression(initExpression, context)
            : null;

        if (variable.Type.MapsToUnknown)
        {
            if (expression is null || expression.Type.MapsToUnknown)
            {
                expression ??= new BoundNeverExpression(syntax, context.Module.Never);
                if (!expression.Type.MapsToNever) expression = expression with { Type = context.Module.Never };
                context.Diagnostics.ReportInvalidImplicitType(syntax.SourceSpan, expression.Type.Name);
            }

            // We've inferred the symbol type, we can replace it with some magic.
            SetType(variable, expression.Type);

            [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(TypeSymbol.Type)}")]
            static extern void SetType(Symbol symbol, TypeSymbol type);
        }
        else
        {
            if (expression is null)
            {
                if (variable.Type.MapsToUnit)
                {
                    expression = new BoundLiteralExpression(SyntaxToken.CreateSynthetic(SyntaxKind.NullKeyword), context.Module.Unit, Unit.Value);
                }
                else
                {
                    context.Diagnostics.ReportUninitializedVariable(syntax.SourceSpan, variable.Name);
                    expression = new BoundNeverExpression(syntax, context.Module.Never);
                }
            }

            expression = expression.Convert(variable.Type, context);
        }

        return new BoundVariableDeclaration(syntax, variable, expression);
    }
}
