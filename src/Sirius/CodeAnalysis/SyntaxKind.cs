namespace Sirius.CodeAnalysis;

public enum SyntaxKind
{
    InvalidToken = 0,
    EndOfFileToken = 1,
    WhitespaceToken = 2,
    NumberToken = 3,
    PlusToken = 4,
    MinusToken = 5,
    StarToken = 6,
    SlashToken = 7,
    OpenParenthesisToken = 8,
    CloseParenthesisToken = 9,
    LiteralExpression = 10,
    BinaryExpression = 11,
    ParenthesizedExpression = 12
}
