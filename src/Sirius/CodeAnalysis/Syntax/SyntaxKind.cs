﻿namespace Sirius.CodeAnalysis.Syntax;

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
    IdentifierToken = 16,

    // Keywords
    TrueKeyword = 14,
    FalseKeyword = 15,

    // Expressions
    LiteralExpression = 10,
    BinaryExpression = 11,
    ParenthesizedExpression = 12,
    UnaryExpression = 13
}