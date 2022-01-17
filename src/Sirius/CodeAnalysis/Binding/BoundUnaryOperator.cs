using Sirius.CodeAnalysis.Syntax;

namespace Sirius.CodeAnalysis.Binding;

internal sealed class BoundUnaryOperator
{
    public Type OperandType { get; }

    public Type Type { get; }

    public SyntaxKind SyntaxKind { get; }

    public BoundUnaryOperatorKind Kind { get; }

    private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType, Type resultType)
    {
        SyntaxKind = syntaxKind;
        Kind = kind;
        OperandType = operandType;
        Type = resultType;
    }

    private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType)
       : this(syntaxKind, kind, operandType, operandType)
    {
    }

    public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, Type operandType)
    {
        foreach (var op in _operators)
        {
            if (op.SyntaxKind == syntaxKind && op.OperandType == operandType)
                return op;
        }

        return null;
    }

    private static BoundUnaryOperator[] _operators =
    {
            new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(int)),
    };
}