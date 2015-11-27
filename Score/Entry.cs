using System;
using System.Diagnostics;
using System.IO;

namespace Score
{
    using Front.Lex;
    using Front.Parse;
    using Debug;
    using Middle;
    using Middle.Symbols;
    using Back;
    using Back.LLVM;

    public static class Entry
    {
        static void Main(string[] args)
        {
            // TODO(kai): compiler state per file pls, makes things easier

            // WHAT WILL HAPPEN, HE ASKS
            var printer = new AstPrinter(Console.Out);

            // I think this is the path we want
            var testFile = "../../../TEST_FILES/lex_test.score";

            var log = new DetailLogger();

            var lexer = new Lexer();
            var tokens = lexer.GetTokens(log, testFile);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            var parser = new Parser();
            var ast = parser.Parse(log, tokens, testFile);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            printer.Visit(ast);

            var symbols = new SymbolTable();
            var analyzer = new SemanticAnalyzer(log, symbols);

            analyzer.Analyze(Path.GetFileNameWithoutExtension(testFile), ast);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            Console.WriteLine();
            Console.WriteLine(symbols);

            var compiler = new ScoreCompiler(log, symbols);
            compiler.Compile(ast);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Compilation successful!");
            Console.WriteLine();
            Console.WriteLine();

            // Run the interpreter!
            Console.WriteLine("Interpreting:");
            var processInfo = new ProcessStartInfo("cmd.exe", @"/c ..\..\..\TEST_FILES\lli.exe ..\..\..\TEST_FILES\test.bc");
            processInfo.CreateNoWindow = false;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = Process.Start(processInfo);
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                Console.WriteLine(e.Data);
            };
            process.BeginOutputReadLine();

            process.WaitForExit();

            Console.ReadLine();
        }

        private static void Fail(DetailLogger log)
        {
            log.Print(Console.Out);
            Console.WriteLine(Environment.NewLine + "Compilation failed.");
            Console.ReadLine();
        }
    }
}
