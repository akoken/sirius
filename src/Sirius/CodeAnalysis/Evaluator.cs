namespace Sirius.CodeAnalysis;

class Evaluator
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
            var left = EvaluateExression(b.Left);
            var right = EvaluateExression(b.Right);

            if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                return left + right;
            else if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                return left - right;
            else if (b.OperatorToken.Kind == SyntaxKind.StarToken)
                return left * right;
            else if (b.OperatorToken.Kind == SyntaxKind.SlashToken)
                return left / right;
            else
                throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
        }

        if (node is ParenthesizedExpressionSyntax p)
        {
            return EvaluateExression(p.Expression);
        }

        throw new Exception($"Unexpected node {node.Kind}");
    }
}