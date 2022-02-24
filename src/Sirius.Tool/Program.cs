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
                foreach (var diagnostic in diagnostics)
                {
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(diagnostic);
                    Console.ResetColor();

                    var prefix = line.Substring(0, diagnostic.Span.Start);
                    var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
                    var suffix = line.Substring(diagnostic.Span.End);

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