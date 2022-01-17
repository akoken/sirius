using Sirius.CodeAnalysis.Syntax;

namespace Sirius.CodeAnalysis.Binding;
internal sealed class BoundBinaryOperator
{
    public Type LeftType { get; }

    public Type RightTye { get; }

    public Type Type { get; }

    private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type leftType, Type rightTye, Type resultType)
    {
        SyntaxKind = syntaxKind;
        Kind = kind;
        LeftType = leftType;
        RightTye = rightTye;
        Type = resultType;
    }

    private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type)
     : this(syntaxKind, kind, type, type, type)
    {

    }

    private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type operandType, Type resultType)
     : this(syntaxKind, kind, operandType, operandType, resultType)
    {

    }

    public SyntaxKind SyntaxKind { get; }
    public BoundBinaryOperatorKind Kind { get; }

    public static BoundBinaryOperator Bind(SyntaxKind syntaxKind, Type leftType, Type rightType)
    {
        foreach (var op in _operators)
        {
            if (op.SyntaxKind == syntaxKind && op.LeftType == leftType && op.RightTye == rightType)
                return op;
        }

        return null;
    }

    private static BoundBinaryOperator[] _operators =
     {
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Substraction, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, typeof(int), typeof(bool)),

            new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, typeof(bool))
    };
}