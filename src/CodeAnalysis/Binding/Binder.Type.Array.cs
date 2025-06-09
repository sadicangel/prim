using System.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static ArraySymbol BindArrayType(ArrayTypeSyntax syntax, BindingContext context)
    {
        var elementType = BindType(syntax.ElementType, context);

        // TODO: This should probably be isz, not i32.
        if (syntax.Length is LiteralExpressionSyntax { SyntaxKind: SyntaxKind.I32LiteralExpression } literal)
        {
            Debug.Assert(literal.InstanceValue is int);

            return new ArraySymbol(syntax, elementType, (int)literal.InstanceValue, context.Module);
        }

        context.Diagnostics.ReportInvalidArrayLength(syntax.Length.SourceSpan);

        return new ArraySymbol(syntax, context.Module.Never, Length: 0, context.Module);
    }
}
