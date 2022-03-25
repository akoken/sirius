using Sirius.CodeAnalysis;
using Sirius.CodeAnalysis.Syntax;
using Sirius.CodeAnalysis.Text;
using System.Text;

namespace Sirius.Tool;

internal static class Program
{
    private static void Main()
    {
        bool showTree = false;
        var showProgram = false;
        var variables = new Dictionary<VariableSymbol, object>();
        var textBuilder = new StringBuilder();
        Compilation previous = null;

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            if (textBuilder.Length == 0)
                Console.Write("» ");
            else
                Console.Write("· ");

            Console.ResetColor();

            string input = Console.ReadLine();
            bool isBlank = string.IsNullOrWhiteSpace(input);

            if (textBuilder.Length == 0)
            {
                if (isBlank)
                {
                    break;
                }
                else if (input == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
                    continue;
                }
                else if (input == "#showProgram")
                {
                    showProgram = !showProgram;
                    Console.WriteLine(showTree ? "Showing bound tree." : "Not showing bound tree.");
                    continue;
                }
                else if (input == "#cls")
                {
                    Console.Clear();
                    continue;
                }
                else if (input == "#reset")
                {
                    previous = null;
                    continue;
                }
            }

            textBuilder.AppendLine(input);
            string text = textBuilder.ToString();

            SyntaxTree syntaxTree = SyntaxTree.Parse(text);

            if (!isBlank && syntaxTree.Diagnostics.Length > 0)
            {
                continue;
            }

            Compilation compilation = previous is null
                ? new Compilation(syntaxTree)
                : previous.ContinueWith(syntaxTree);

            EvaluationResult result = compilation.Evaluate(variables);
            IReadOnlyList<Diagnostic> diagnostics = result.Diagnostics;

            if (showTree)
                syntaxTree.Root.WriteTo(Console.Out);

            if (showProgram)
                compilation.EmitTree(Console.Out);

            if (!diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(result.Value);
                Console.ResetColor();

                previous = compilation;
            }
            else
            {
                foreach (var diagnostic in diagnostics)
                {
                    var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                    var line = syntaxTree.Text.Lines[lineIndex];
                    var lineNumber = lineIndex + 1;
                    int character = diagnostic.Span.Start - line.Start + 1;
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write($"({lineNumber}, {character}):");
                    Console.WriteLine(diagnostic);
                    Console.ResetColor();

                    var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                    var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                    var prefix = syntaxTree.Text.ToString(prefixSpan);
                    var error = syntaxTree.Text.ToString(diagnostic.Span);
                    var suffix = syntaxTree.Text.ToString(suffixSpan);

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

            textBuilder.Clear();
        }
    }
}