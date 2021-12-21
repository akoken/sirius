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

            var lexer = new Lexer(line);
            while (true)
            {
                var token = lexer.NextToken();
                if (token.Type == TokenType.EOF)
                    break;

                Console.Write($"{token.Type}: '{token.Text}'");
                if (token.Value is not null)
                    Console.Write($"{token.Value}");

                Console.WriteLine();
            }
        }
    }
}

enum TokenType
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
    Invalid
}

class SyntaxToken
{
    public SyntaxToken(TokenType type, int position, string text, object value)
    {
        Type = type;
        Position = position;
        Text = text;
        Value = value;
    }

    public TokenType Type { get; }
    public int Position { get; }
    public string Text { get; }
    public object Value { get; }
}

class Lexer
{
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
            return new SyntaxToken(TokenType.EOF, _position, "\0", null);

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
            return new SyntaxToken(TokenType.Number, start, text, value);
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
            return new SyntaxToken(TokenType.Whitespace, start, text, null);
        }

        if (Current == '+')
            return new SyntaxToken(TokenType.Plus, _position++, "+", null);
        else if (Current == '-')
            return new SyntaxToken(TokenType.Minus, _position++, "-", null);
        else if (Current == '*')
            return new SyntaxToken(TokenType.Star, _position++, "*", null);
        else if (Current == '/')
            return new SyntaxToken(TokenType.Slash, _position++, "/", null);
        else if (Current == '(')
            return new SyntaxToken(TokenType.OpenParenthesis, _position++, "(", null);
        else if (Current == ')')
            return new SyntaxToken(TokenType.CloseParenthesis, _position++, ")", null);

        return new SyntaxToken(TokenType.Invalid, _position++, _text.Substring(_position - 1, 1), null);
    }
}