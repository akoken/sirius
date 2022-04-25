using System.Collections.Immutable;
using Sirius.CodeAnalysis.Symbols;

namespace Sirius.CodeAnalysis.Binding;

internal sealed class BoundCallExpression : BoundExpression
{

    public BoundCallExpression(FunctionSymbol function, ImmutableArray<BoundExpression> arguments)
    {
        Function = function;
        Arguments = arguments;
    }

    public override TypeSymbol Type => Function.Type;

    public override BoundNodeKind Kind => BoundNodeKind.CallExpression;

    public FunctionSymbol Function { get; }
    public ImmutableArray<BoundExpression> Arguments { get; }
}
