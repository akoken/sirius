namespace Sirius;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Write(">");
            string line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                return;

            var parser = new Parser(line);
            var expression = parser.Parse();

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            PrettyPrint(expression);
            Console.ForegroundColor = color;

            if (parser.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                foreach (var diagnostic in parser.Diagnostics)
                {
                    Console.WriteLine(diagnostic);
                }
                Console.ForegroundColor = color;
            }
        }
    }

    static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        Console.Write(indent);
        Console.Write(marker);
        Console.Write(node.Kind);

        if (node is SyntaxToken t && t.Value != null)
        {
            Console.Write(" ");
            Console.Write(t.Value);
        }

        Console.WriteLine();

        indent += isLast ? "    " : "│   ";

        var lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
        {
            PrettyPrint(child, indent, child == lastChild);
        }
    }
}

enum SyntaxKind
{
    EOF,
    Number,
    Whitespace,
    Plus,
    Minus,
    Star,
    Slash,
    OpenParenthesis,
    CloseParenthesis,
    Invalid,
    BinaryExpression
}

class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, int position, string text, object value)
    {
        Kind = kind;
        Position = position;
        Text = text;
        Value = value;
    }

    public override SyntaxKind Kind { get; }
    public int Position { get; }
    public string Text { get; }
    public object Value { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Enumerable.Empty<SyntaxNode>();
    }
}

class Lexer
{
    private List<string> _diagnostics = new();
    private readonly string _text;
    private int _position;

    private char Current
    {
        get
        {
            if (_position >= _text.Length)
                return '\0';
            return _text[_position];
        }
    }

    public IEnumerable<string> Diagnostics => _diagnostics;

    public Lexer(string text)
    {
        _text = text;
    }

    private void Next()
    {
        _position++;
    }

    public SyntaxToken NextToken()
    {
        if (_position >= _text.Length)
        {
            return new SyntaxToken(SyntaxKind.EOF, _position, "\0", null);
        }

        if (char.IsDigit(Current))
        {
            var start = _position;

            while (char.IsDigit(Current))
            {
                Next();
            }

            var length = _position - start;
            var text = _text.Substring(start, length);
            int.TryParse(text, out int value);

            return new SyntaxToken(SyntaxKind.Number, start, text, value);
        }

        if (char.IsWhiteSpace(Current))
        {
            var start = _position;

            while (char.IsWhiteSpace(Current))
            {
                Next();
            }

            var length = _position - start;
            var text = _text.Substring(start, length);
            return new SyntaxToken(SyntaxKind.Whitespace, start, text, null);
        }

        if (Current == '+')
            return new SyntaxToken(SyntaxKind.Plus, _position++, "+", null);
        else if (Current == '-')
            return new SyntaxToken(SyntaxKind.Minus, _position++, "-", null);
        else if (Current == '*')
            return new SyntaxToken(SyntaxKind.Star, _position++, "*", null);
        else if (Current == '/')
            return new SyntaxToken(SyntaxKind.Slash, _position++, "/", null);
        else if (Current == '(')
            return new SyntaxToken(SyntaxKind.OpenParenthesis, _position++, "(", null);
        else if (Current == ')')
            return new SyntaxToken(SyntaxKind.CloseParenthesis, _position++, ")", null);

        _diagnostics.Add($"Error: Bad character input: {Current}");
        return new SyntaxToken(SyntaxKind.Invalid, _position++, _text.Substring(_position - 1, 1), null);
    }
}

abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }

    public abstract IEnumerable<SyntaxNode> GetChildren();
}

abstract class ExpressionSyntax : SyntaxNode
{
}

sealed class NumberExpressionSyntax : ExpressionSyntax
{
    public NumberExpressionSyntax(SyntaxToken numberToken)
    {
        NumberToken = numberToken;
    }

    public SyntaxToken NumberToken { get; }

    public override SyntaxKind Kind => NumberToken.Kind;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return NumberToken;
    }
}

sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public ExpressionSyntax Left { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }

    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}

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

            if (token.Kind != SyntaxKind.Whitespace && token.Kind != SyntaxKind.Invalid)
            {
                tokens.Add(token);
            }
        } while (token.Kind != SyntaxKind.EOF);

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

    public ExpressionSyntax Parse()
    {
        var left = ParsePrimaryExpression();

        while (Current.Kind == SyntaxKind.Plus || Current.Kind == SyntaxKind.Minus)
        {
            var operatorToken = NextToken();
            var right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }
        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        var numberToken = Match(SyntaxKind.Number);
        return new NumberExpressionSyntax(numberToken);
    }
}