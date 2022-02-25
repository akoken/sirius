using Sirius.CodeAnalysis;
using Sirius.CodeAnalysis.Syntax;

namespace Sirius.Tool;

internal static class Program
{
    private static void Main()
    {
        bool showTree = false;
        var variables = new Dictionary<VariableSymbol, object>();
        while (true)
        {
            Console.Write(">");

            string line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            if (line == "#showTree")
            {
                showTree = !showTree;
                Console.WriteLine(showTree ? "Showing parse trees..." : "Not showing parse trees...");
                continue;
            }
            else if (line == "#cls")
            {
                Console.Clear();
                continue;
            }

            SyntaxTree syntaxTree = SyntaxTree.Parse(line);
            var compilation = new Compilation(syntaxTree);
            EvaluationResult result = compilation.Evaluate(variables);
            IReadOnlyList<Diagnostic> diagnostics = result.Diagnostics;

            if (showTree)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                syntaxTree.Root.WriteTo(Console.Out);
                Console.ResetColor();
            }

            if (!diagnostics.Any())
            {
                Console.WriteLine(result.Value);
            }
            else
            {
                var text = syntaxTree.Text;
                foreach (var diagnostic in diagnostics)
                {
                    var lineIndex = text.GetLineIndex(diagnostic.Span.Start);
                    var lineNumber = lineIndex + 1;
                    var character = diagnostic.Span.Start - text.Lines[lineIndex].Start + 1;
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write($"({lineNumber}, {character}):");
                    Console.WriteLine(diagnostic);
                    Console.ResetColor();

                    var prefix = line.AsSpan(0, diagnostic.Span.Start).ToString();
                    var error = line.AsSpan(diagnostic.Span.Start, diagnostic.Span.Length).ToString();
                    var suffix = line.AsSpan(diagnostic.Span.End).ToString();

                    Console.Write("    ");
                    Console.Write(prefix);

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(error);
                    Console.ResetColor();

                    Console.Write(suffix);
                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }
    }
}