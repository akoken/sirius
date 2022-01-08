namespace Sirius.CodeAnalysis.Syntax;

public enum SyntaxKind
{
    // Tokens
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
    IdentifierToken = 10,

    // Keywords
    TrueKeyword = 11,
    FalseKeyword = 12,

    // Expressions
    LiteralExpression = 13,
    BinaryExpression = 14,
    ParenthesizedExpression = 15,
    UnaryExpression = 16
}
