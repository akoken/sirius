namespace Sirius.CodeAnalysis;

public enum SyntaxKind
{
    InvalidToken = 0,
    EndOfFileToken = 1,
    NumberToken = 2,
    WhitespaceToken = 3,
    PlusToken = 4,
    MinusToken = 5,
    StarToken = 6,
    SlashToken = 7,
    OpenParenthesisToken = 8,
    CloseParenthesisToken = 9,
    BinaryExpressionToken = 10,
    ParenthesizedExpression = 11
}
