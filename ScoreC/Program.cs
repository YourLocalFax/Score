﻿using System;

using Dbg;
using Lex;
using Log;
using Parse;
using TyChecker;
using Semantics;

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

            var fileName = args[0];

            var log = new DetailLogger();

            var lexer = new Lexer();
            var tokens = lexer.GetTokens(log, fileName);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            var parser = new Parser();
            var ast = parser.Parse(log, tokens, fileName);

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
