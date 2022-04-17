using Sirius.CodeAnalysis.Symbols;

namespace Sirius.CodeAnalysis.Binding;

internal sealed class BoundLiteralExpression : BoundExpression
{
    public object Value { get; }

    public BoundLiteralExpression(object value)
    {
        Value = value;
        Type = value switch
        {
            bool => TypeSymbol.Bool,
            int => TypeSymbol.Int,
            string => TypeSymbol.String,
            _ => throw new Exception($"Unexpected literal '{value}' of type {value.GetType()}"),
        };
    }

    public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;

    public override TypeSymbol Type { get; }
}
