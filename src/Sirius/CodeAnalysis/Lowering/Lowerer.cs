using Sirius.CodeAnalysis.Binding;
using System.Collections.Immutable;

namespace Sirius.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private Lowerer() { }

        public static BoundStatement Lower(BoundStatement statement)
        {
            Lowerer lowerer = new();
            return lowerer.RewriteStatement(statement);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.LowerBound);
            BoundVariableExpression variableExpression = new(node.Variable);
            var condition = new BoundBinaryExpression(variableExpression, BoundBinaryOperator.Bind(Syntax.SyntaxKind.LessOrEqualsToken, typeof(int), typeof(int)), node.UpperBound);
            var increment = new BoundExpressionStatement(new BoundAssignmentExpression(node.Variable, new BoundBinaryExpression(variableExpression, BoundBinaryOperator.Bind(Syntax.SyntaxKind.PlusToken, typeof(int), typeof(int)), new BoundLiteralExpression(1))));

            var whileBody = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.Body, increment));
            var whileStatement = new BoundWhileStatement(condition, whileBody);
            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(variableDeclaration, whileStatement));

            return RewriteStatement(result);
        }
    }
}
