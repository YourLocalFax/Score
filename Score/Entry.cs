﻿using System;
using System.Diagnostics;
using System.IO;

namespace Score
{
    using Front.Lex;
    using Front.Parse;
    using Front.Parse.Ty;
    using Front.Parse.Data;
    using Debug;
    using Middle.Analysis;
    using Middle.Symbols;
    using Back;

    internal static class Tests
    {
        static void TestSymbolTable()
        {
            var symbols = new SymbolTable();

            symbols.Insert("test_kit", Symbol.Kind.KIT, null, null);
            symbols.NewScope("test_kit"); {

                symbols.Insert("main", Symbol.Kind.FN, new TyFn(new ParameterList(),
                    new Parameter(null, TyRef.VoidTy), null), new Modifiers());
                symbols.NewScope("main"); {
                    symbols.Insert("test", Symbol.Kind.VAR, TyRef.BoolTy, new Modifiers());
                } symbols.ExitScope();

                var putsParamTys = new ParameterList();
                putsParamTys.Add(new Parameter(null, TyRef.PointerTo(TyRef.Int8Ty, false)));
                var putsTy = new TyFn(putsParamTys, new Parameter(null, TyRef.Int32Ty), null);
                symbols.Insert("puts", Symbol.Kind.FN, putsTy, new Modifiers());

            } symbols.ExitScope();

            Console.WriteLine(symbols);

            var walker = new SymbolTableWalker(symbols);
            while (walker.Current != null)
            {
                Console.WriteLine(walker.Current);
                walker.Step();
            }

            Entry.Wait();
        }
    }

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

            /*
            Wait();
            return;
            */

            Console.WriteLine();
            Console.WriteLine("COMPILING:");

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

            Wait();
        }

        public static void Wait()
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
