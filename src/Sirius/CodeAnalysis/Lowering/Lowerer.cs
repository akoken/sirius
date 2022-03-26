using Sirius.CodeAnalysis.Binding;

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
    }
}
