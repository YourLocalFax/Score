//#define COMPILE

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score
{
    using Front.Lex;
    using Front.Parse;
    using Front.Parse.Ty;
    using Front.Parse.Data;
    using Debug;
    using Middle.Analysis;
    using Middle.Symbols;
    using Middle.FnPopulation;
    using Middle.TypeCheck;
    using Middle.TypeResolve;
    using Back;

    internal static class Tests
    {
        /*
        static void TestSymbolTable()
        {
            var symbols = new SymbolTable();

            symbols.Insert("test_kit", Symbol.Kind.KIT, null, null);
            symbols.NewScope("test_kit"); {

                symbols.Insert("main", Symbol.Kind.FN, new TyFn(new ParameterList(),
                    new Parameter(null, TyRef.VoidTy)), new Modifiers());
                symbols.NewScope("main"); {
                    symbols.Insert("test", Symbol.Kind.VAR, TyRef.BoolTy, new Modifiers());
                } symbols.ExitScope();

                var putsParamTys = new ParameterList();
                putsParamTys.Add(new Parameter(null, TyRef.PointerTo(TyRef.Int8Ty, false)));
                var putsTy = new TyFn(putsParamTys, new Parameter(null, TyRef.Int32Ty));
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
        //*/
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
            var manager = new GlobalStateManager(ContextCreate());

            var analyzer = new SemanticAnalyzer(log, symbols, manager);
            var module = analyzer.Analyze(Path.GetFileNameWithoutExtension(testFile), ast);

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
            Console.WriteLine("TYPE RESOLVING:");

            var typeResolver = new TypeResolver(log);
            typeResolver.Resolve(ast, symbols);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            Console.WriteLine();
            Console.WriteLine("FUNCTION POPULATING:");

            var fnPopulator = new FnPopulator(log, manager, module);
            fnPopulator.Populate(ast, symbols);

            AddPutB(symbols, manager, module);
            AddPutI(symbols, manager, module);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            Console.WriteLine();
            Console.WriteLine("TYPE CHECKING:");

            var typeChecker = new TypeChecker(log, symbols);
            typeChecker.Check(ast);

            if (log.HasErrors)
            {
                Fail(log);
                return;
            }

            Console.WriteLine();
            Console.WriteLine("COMPILING:");

            var compiler = new ScoreCompiler(log, symbols, manager, module);
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

#if COMPILE
            // Compile the code!

            InitializeX86TargetInfo();
            InitializeX86Target();
            InitializeX86TargetMC();

            IntPtr err;

            var target = GetTargetFromName("x86");
            if (target.Pointer == IntPtr.Zero)
            {
                Console.WriteLine("Das not pretty neat.");
                Wait();
                return;
            }

            var targetMachine = CreateTargetMachine(target, "x86_64-pc-windows-gnu", "x86-64", "",
                LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault, LLVMRelocMode.LLVMRelocDefault, LLVMCodeModel.LLVMCodeModelDefault);

            var name = Marshal.StringToHGlobalAnsi("../../../TEST_FILES/test.o");
            TargetMachineEmitToFile(targetMachine, compiler.module, name, LLVMCodeGenFileType.LLVMObjectFile, out err);

            Console.WriteLine(Marshal.PtrToStringAnsi(err));
#else
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
#endif

            Wait();
        }

        private static void AddPutB(SymbolTable symbols, GlobalStateManager manager, LLVMModuleRef module)
        {
            var puts = (symbols.global.Lookup("puts") as FnSymbol).llvmFn;

            var putbTy = FunctionType(VoidTypeInContext(manager.context), new LLVMTypeRef[] { Int1TypeInContext(manager.context) }, false);
            var putb = AddFunction(module, "putb", putbTy);

            var putbScParams = new ParameterList();
            putbScParams.Add(new Parameter(null, new Front.Spanned<TyRef>(default(Front.Span), TyRef.BoolTy)));
            symbols.InsertFn("putb", new Modifiers(), new TyFn(putbScParams,
                new Parameter(null, new Front.Spanned<TyRef>(default(Front.Span), TyRef.VoidTy))));
            (symbols.global.Lookup("putb") as FnSymbol).llvmFn = putb;

            var bParam = GetParam(putb, 0);
            SetValueName(bParam, "b");

            var builder = CreateBuilderInContext(manager.context);

            var _entry = AppendBasicBlockInContext(manager.context, putb, ".entry");
            var _if = AppendBasicBlockInContext(manager.context, putb, ".if");
            var _next = AppendBasicBlockInContext(manager.context, putb, ".next");
            var _exit = AppendBasicBlockInContext(manager.context, putb, ".exit");

            PositionBuilderAtEnd(builder, _entry);
            BuildCondBr(builder, bParam, _if, _next);

            PositionBuilderAtEnd(builder, _if);
            BuildCall(builder, puts, new LLVMValueRef[] { manager.CStrConst(builder, "true") }, "");
            BuildBr(builder, _exit);

            PositionBuilderAtEnd(builder, _next);
            BuildCall(builder, puts, new LLVMValueRef[] { manager.CStrConst(builder, "false") }, "");
            BuildBr(builder, _exit);

            PositionBuilderAtEnd(builder, _exit);
            BuildRetVoid(builder);
        }

        private static void AddPutI(SymbolTable symbols, GlobalStateManager manager, LLVMModuleRef module)
        {
            /*
            var putcharTy = FunctionType(Int32TypeInContext(manager.context), new LLVMTypeRef[] { Int32TypeInContext(manager.context) }, false);
            var putchar = AddFunction(module, "putchar", putcharTy);
            SetLinkage(putchar, LLVMLinkage.LLVMExternalLinkage);
            */

            var putchar = (symbols.global.Lookup("putchar") as FnSymbol).llvmFn;

            var putiTy = FunctionType(VoidTypeInContext(manager.context), new LLVMTypeRef[] { Int32TypeInContext(manager.context) }, false);
            var puti = AddFunction(module, "puti", putiTy);

            var putiScParams = new ParameterList();
            putiScParams.Add(new Parameter(null, new Front.Spanned<TyRef>(default(Front.Span), TyRef.Int32Ty)));
            symbols.InsertFn("puti", new Modifiers(), new TyFn(putiScParams,
                new Parameter(null, new Front.Spanned<TyRef>(default(Front.Span), TyRef.VoidTy))));
            (symbols.global.Lookup("puti") as FnSymbol).llvmFn = puti;

            var iParam = GetParam(puti, 0);
            SetValueName(iParam, "i");

            var builder = CreateBuilderInContext(manager.context);

            var _entry = AppendBasicBlockInContext(manager.context, puti, ".entry");
            var _if0 = AppendBasicBlockInContext(manager.context, puti, ".if0");
            var _next0 = AppendBasicBlockInContext(manager.context, puti, ".next0");
            var _if1 = AppendBasicBlockInContext(manager.context, puti, ".if1");
            var _next1 = AppendBasicBlockInContext(manager.context, puti, ".next1");

            PositionBuilderAtEnd(builder, _entry);
            var i = BuildAlloca(builder, Int32TypeInContext(manager.context), "i_loc");
            BuildStore(builder, iParam, i);

            var ilt0 = BuildICmp(builder, LLVMIntPredicate.LLVMIntSLT, BuildLoad(builder, i, ""), ConstInt(Int32TypeInContext(manager.context), 0, true), "");
            BuildCondBr(builder, ilt0, _if0, _next0);

            PositionBuilderAtEnd(builder, _if0);
            BuildCall(builder, putchar, new LLVMValueRef[] { ConstInt(Int32TypeInContext(manager.context), 45, true) }, "");
            BuildStore(builder, BuildNeg(builder, BuildLoad(builder, i, ""), ""), i);
            BuildBr(builder, _next0);

            PositionBuilderAtEnd(builder, _next0);
            var igt9 = BuildICmp(builder, LLVMIntPredicate.LLVMIntSGT, BuildLoad(builder, i, ""), ConstInt(Int32TypeInContext(manager.context), 9, true), "");
            BuildCondBr(builder, igt9, _if1, _next1);

            PositionBuilderAtEnd(builder, _if1);
            var io10 = BuildSDiv(builder, BuildLoad(builder, i, ""), ConstInt(Int32TypeInContext(manager.context), 10, true), "");
            BuildCall(builder, puti, new LLVMValueRef[] { io10 }, "");
            BuildBr(builder, _next1);

            PositionBuilderAtEnd(builder, _next1);
            var im10 = BuildSRem(builder, BuildLoad(builder, i, ""), ConstInt(Int32TypeInContext(manager.context), 10, true), "");
            var c0pim10 = BuildAdd(builder, ConstInt(Int32TypeInContext(manager.context), 48, true), im10, "");

            BuildCall(builder, putchar, new LLVMValueRef[] { c0pim10 }, "");
            BuildRetVoid(builder);
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
