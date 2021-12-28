namespace Sirius.CodeAnalysis;

sealed class NumberExpressionSyntax : ExpressionSyntax
{
    public NumberExpressionSyntax(SyntaxToken numberToken)
    {
        NumberToken = numberToken;
    }

    public SyntaxToken NumberToken { get; }

    public override SyntaxKind Kind => NumberToken.Kind;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return NumberToken;
    }
}
