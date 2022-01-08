using Sirius.CodeAnalysis.Binding;

namespace Sirius.CodeAnalysis;

internal sealed class Evaluator
{
    private readonly BoundExpression _root;

    public Evaluator(BoundExpression root)
    {
        _root = root;
    }

    public object Evaluate()
    {
        return EvaluateExression(_root);
    }

    private object EvaluateExression(BoundExpression node)
    {
        if (node is BoundLiteralExpression n)
        {
            return n.Value;
        }

        if (node is BoundUnaryExpression u)
        {
            int operand = (int)EvaluateExression(u.Operand);
            switch (u.OperatorKind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return operand;
                case BoundUnaryOperatorKind.Negation:
                    return -operand;
                default:
                    throw new Exception($"Unexpected unary operator {u.OperatorKind}");
            }
        }

        if (node is BoundBinaryExpression b)
        {
            int left = (int)EvaluateExression(b.Left);
            int right = (int)EvaluateExression(b.Right);

            switch (b.OperatorKind)
            {
                case BoundBinaryOperatorKind.Addition:
                    return left + right;
                case BoundBinaryOperatorKind.Substraction:
                    return left - right;
                case BoundBinaryOperatorKind.Multiplication:
                    return left * right;
                case BoundBinaryOperatorKind.Division:
                    return left / right;
                default:
                    throw new Exception($"Unexpected binary operator {b.OperatorKind}");
            }
        }

        throw new Exception($"Unexpected node {node.Kind}");
    }
}