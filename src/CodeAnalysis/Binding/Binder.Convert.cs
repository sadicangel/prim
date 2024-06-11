using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Binding.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression Convert(BoundExpression expression, PrimType type, bool isExplicit, BindingContext context)
    {
        if (type.IsAny)
            return expression;

        if (expression.Type == type)
        {
            if (isExplicit)
            {
                context.Diagnostics.ReportRedundantConversion(expression.SyntaxNode.Location);
            }
            return expression;
        }

        var conversion = expression.Type.GetConversion(expression.Type, type)
            ?? type.GetConversion(expression.Type, type);

        if (conversion is null)
        {
            context.Diagnostics.ReportInvalidConversion(expression.SyntaxNode.Location, expression.Type.Name, type.Name);
            return new BoundNeverExpression(expression.SyntaxNode);
        }

        if (!isExplicit && conversion.IsExplicit)
        {
            context.Diagnostics.ReportInvalidImplicitConversion(expression.SyntaxNode.Location, expression.Type.Name, type.Name);
            return new BoundNeverExpression(expression.SyntaxNode);
        }

        var conversionSymbol = new ConversionSymbol(expression.SyntaxNode, conversion);

        return new BoundConversionExpression(expression.SyntaxNode, conversionSymbol, expression);
    }
}
