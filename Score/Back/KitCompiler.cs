using LLVMSharp;

namespace Score.Back
{
    using Front.Parse;
    using Front.Parse.SyntaxTree;

    using Middle.Symbols;

    /// <summary>
    /// Compiles entire programs.
    /// </summary>
    internal sealed class KitCompiler : ModCompiler
    {
        public KitCompiler(DetailLogger log, GlobalStateManager manager, LLVMModuleRef module, SymbolTableWalker walker)
            : base(log, manager, module, walker)
        {
        }
    }
}
