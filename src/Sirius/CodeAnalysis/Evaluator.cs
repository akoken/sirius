namespace Sirius.CodeAnalysis;

public class Evaluator
{
    private readonly ExpressionSyntax _root;

    public Evaluator(ExpressionSyntax root)
    {
        _root = root;
    }

    public int Evaluate()
    {
        return EvaluateExression(_root);
    }

    private int EvaluateExression(ExpressionSyntax node)
    {
        if (node is NumberExpressionSyntax n)
        {
            return (int)n.NumberToken.Value;
        }

        if (node is BinaryExpressionSyntax b)
        {
            int left = EvaluateExression(b.Left);
            int right = EvaluateExression(b.Right);

            switch (b.OperatorToken.Kind)
            {
                case SyntaxKind.PlusToken:
                    return left + right;
                case SyntaxKind.MinusToken:
                    return left - right;
                case SyntaxKind.StarToken:
                    return left * right;
                case SyntaxKind.SlashToken:
                    return left / right;
                default:
                    throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
            }
        }

        if (node is ParenthesizedExpressionSyntax p)
        {
            return EvaluateExression(p.Expression);
        }

        throw new Exception($"Unexpected node {node.Kind}");
    }
}