namespace Sirius.CodeAnalysis.Syntax;

public enum SyntaxKind
{
    // Tokens
    InvalidToken,
    EndOfFileToken,
    WhitespaceToken,
    NumberToken,
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    BangToken,
    EqualsToken,
    AmpersandAmpersandToken,
    PipePipeToken,
    EqualsEqualsToken,
    BangEqualsToken,
    OpenParenthesisToken,
    CloseParenthesisToken,
    OpenBraceToken,
    CloseBraceToken,
    IdentifierToken,

    // Keywords    
    FalseKeyword,
    LetKeyword,
    TrueKeyword,
    VarKeyword,

    // Nodes
    CompilationUnit,

    // Statements
    BlockStatement,
    ExpressionStatement,
    VariableDeclaration,

    // Expressions
    LiteralExpression,
    NameExpression,
    UnaryExpression,
    BinaryExpression,
    ParenthesizedExpression,
    AssignmentExpression,
}
