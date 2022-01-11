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
            var operand = EvaluateExression(u.Operand);
            switch (u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
                default:
                    throw new Exception($"Unexpected unary operator {u.Op}");
            }
        }

        if (node is BoundBinaryExpression b)
        {
            var left = EvaluateExression(b.Left);
            var right = EvaluateExression(b.Right);

            switch (b.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    return (int)left + (int)right;
                case BoundBinaryOperatorKind.Substraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorKind.Division:
                    return (int)left / (int)right;
                case BoundBinaryOperatorKind.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.LogicalOr:
                    return (bool)left || (bool)right;
                default:
                    throw new Exception($"Unexpected binary operator {b.Op}");
            }
        }

        throw new Exception($"Unexpected node {node.Kind}");
    }
}