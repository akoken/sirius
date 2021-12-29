namespace Sirius.CodeAnalysis;

public sealed class NumberExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken NumberToken { get; }

    public NumberExpressionSyntax(SyntaxToken numberToken)
    {
        NumberToken = numberToken;
    }

    public override SyntaxKind Kind => NumberToken.Kind;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return NumberToken;
    }
}
