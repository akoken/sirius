using System.Collections.Immutable;

namespace Sirius.CodeAnalysis.Binding;

internal sealed class BoundBlockStatement : BoundStatement
{
    public BoundBlockStatement(ImmutableArray<BoundStatement> statements)
    {
        Statements = statements;
    }

    public ImmutableArray<BoundStatement> Statements { get; }

    public override BoundNodeKind Kind => BoundNodeKind.BlockStatement;
}
