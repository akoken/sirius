namespace Sirius.CodeAnalysis.Binding;

internal enum BoundNodeKind
{
    // Statements
    BlockStatement,
    VariableDeclaration,
    ExpressionStatement,

    // Expressions
    UnaryExpression,
    LiteralExpression,
    BinaryExpression,
    VariableExpression,
    AssignmentExpression,
}
