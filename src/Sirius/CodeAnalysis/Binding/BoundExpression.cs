using Sirius.CodeAnalysis.Symbols;

namespace Sirius.CodeAnalysis.Binding;

internal abstract class BoundExpression : BoundNode
{
    public abstract TypeSymbol Type { get; }
}
