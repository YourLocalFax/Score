using LLVMSharp;

namespace Score.Middle.Analysis
{
    using Back;

    using Symbols;

    internal sealed class KitAnalyzer : ModAnalyzer
    {
        public KitAnalyzer(DetailLogger log, SymbolTable symbols, GlobalStateManager manager, LLVMModuleRef module)
            : base(log, symbols, manager, module)
        {
        }
    }
}
