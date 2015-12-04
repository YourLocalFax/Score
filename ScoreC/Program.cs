using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lex;
using Log;
using Source;

namespace ScoreC
{
    static class Program
    {
        static void Main(string[] args)
        {
            var fileName = @"D:\projects\c#\Score\TEST_FILES\lex_test.score";

            var log = new DetailLogger();

            var lexer = new Lexer();
            var tokens = lexer.GetTokens(log, fileName);

            while (tokens.HasCurrent)
            {
                Console.WriteLine(tokens.Current.value);
                tokens.Advance();
            }

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
