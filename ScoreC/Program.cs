using System;
using System.Diagnostics;
using System.IO;

using CodeGen;
using Dbg;
using Lex;
using Log;
using Parse;
using TyChecker;
using Semantics;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace ScoreC
{
    static class Program
    {
        static void Main(string[] args)
        {
            // TODO(kai): parse out the args and do things

            if (args.Length == 0)
            {
                Console.WriteLine("No file passed to compile.");
                Wait();
                return;
            }

            var filePath = args[0];
            var dir = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            var log = new DetailLogger();

            var lexer = new Lexer();
            var tokens = lexer.GetTokens(log, filePath);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            var parser = new Parser();
            var ast = parser.Parse(log, tokens, filePath);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            var writer = new AstWriter(Console.Out);
            writer.Visit(ast);

            var semantics = new SemanticAnalyzer(log);
            var symbols = semantics.Analyze(ast);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            Console.WriteLine(symbols);

            var tyChecker = new TypeChecker(log);
            tyChecker.Check(ast, symbols);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            var compiler = new ScoreCompiler(log);
            var module = compiler.Compile(fileName, ast, symbols);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            DumpModule(module);
            var bcFilePath = Path.Combine(dir, fileName + ".bc");
            WriteBitcodeToFile(module, bcFilePath);

            // Run the interpreter!
            Console.WriteLine();
            Console.WriteLine("Interpreting:");
            var cmd = @"/c ..\..\..\..\TEST_FILES\lli.exe " + bcFilePath;
            //Console.WriteLine(cmd);
            var processInfo = new ProcessStartInfo("cmd.exe", cmd);
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

            Wait();
        }

        private static void Wait()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void Fail(DetailLogger log)
        {
            log.Print(Console.Out);
            Console.WriteLine(Environment.NewLine + "Compilation failed.");
            Console.ReadLine();
        }
    }
}
