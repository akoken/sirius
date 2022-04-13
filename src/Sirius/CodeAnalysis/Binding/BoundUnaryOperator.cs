using Sirius.CodeAnalysis.Symbols;
using Sirius.CodeAnalysis.Syntax;

namespace Sirius.CodeAnalysis.Binding;

internal sealed class BoundUnaryOperator
{
    public TypeSymbol OperandType { get; }

    public TypeSymbol Type { get; }

    public SyntaxKind SyntaxKind { get; }

    public BoundUnaryOperatorKind Kind { get; }

    private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
    {
        SyntaxKind = syntaxKind;
        Kind = kind;
        OperandType = operandType;
        Type = resultType;
    }

    private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType)
       : this(syntaxKind, kind, operandType, operandType)
    {
    }

    public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol operandType)
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
            new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, TypeSymbol.Bool),
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.OnesComplement, TypeSymbol.Int),
    };
}
