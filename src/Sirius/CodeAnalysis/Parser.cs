namespace Sirius.CodeAnalysis;

class Parser
{
    private readonly SyntaxToken[] _tokens;
    private int _position;
    private List<string> _diagnostics = new();

    public Parser(string text)
    {
        var tokens = new List<SyntaxToken>();
        var lexer = new Lexer(text);
        SyntaxToken token;

        do
        {
            token = lexer.NextToken();

            if (token.Kind != SyntaxKind.WhitespaceToken && token.Kind != SyntaxKind.InvalidToken)
            {
                tokens.Add(token);
            }
        } while (token.Kind != SyntaxKind.EOFToken);

        _tokens = tokens.ToArray();
        _diagnostics.AddRange(lexer.Diagnostics);
    }

    private SyntaxToken Peek(int offset = 0)
    {
        var index = _position + offset;
        if (index >= _tokens.Length)
        {
            return _tokens[_tokens.Length - 1];
        }

        return _tokens[index];
    }

    public IEnumerable<string> Diagnostics => _diagnostics;
    private SyntaxToken Current => Peek(0);

    private SyntaxToken NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }

    private SyntaxToken Match(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return NextToken();

        _diagnostics.Add($"Error: Unexpected token <{Current.Kind}>, expected <{kind}>");
        return new SyntaxToken(kind, Current.Position, null, null);
    }

    private ExpressionSyntax ParseExpression()
    {
        return ParseTerm();
    }

    public SyntaxTree Parse()
    {
        var expression = ParseTerm();
        var endOfFileToken = Match(SyntaxKind.EOFToken);
        return new SyntaxTree(_diagnostics, expression, endOfFileToken);
    }

    private ExpressionSyntax ParseTerm()
    {
        ExpressionSyntax? left = ParseFactor();

        while (Current.Kind == SyntaxKind.PlusToken || Current.Kind == SyntaxKind.MinusToken)
        {
            SyntaxToken? operatorToken = NextToken();
            ExpressionSyntax? right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParseFactor()
    {
        ExpressionSyntax? left = ParsePrimaryExpression();

        while (Current.Kind == SyntaxKind.StarToken || Current.Kind == SyntaxKind.SlashToken)
        {
            SyntaxToken? operatorToken = NextToken();
            ExpressionSyntax? right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        if (Current.Kind == SyntaxKind.OpenParenthesisToken)
        {
            var left = NextToken();
            var expression = ParseExpression();
            var right = Match(SyntaxKind.CloseParenthesisToken);

            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        SyntaxToken? numberToken = Match(SyntaxKind.NumberToken);
        return new NumberExpressionSyntax(numberToken);
    }
}
