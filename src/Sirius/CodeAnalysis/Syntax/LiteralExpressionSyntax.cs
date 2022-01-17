namespace Sirius.CodeAnalysis.Syntax;

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken LiteralToken { get; }

    public object Value { get; }

    public LiteralExpressionSyntax(SyntaxToken literalToken, object value)
    {
        LiteralToken = literalToken;
        Value = value;
    }

    public LiteralExpressionSyntax(SyntaxToken literalToken)
        : this(literalToken, literalToken.Value)
    {
    }

    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return LiteralToken;
    }
}