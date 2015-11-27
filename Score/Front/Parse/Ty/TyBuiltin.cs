namespace Score.Front.Parse.Ty
{
    using Lex;

    /// <summary>
    /// Represents a built-in type in the Score language.
    /// 
    /// The built-in names include
    /// <code>i8, u8, i16, u16, i32, u32, i64, u64, f16, f32, f64, bool</code>
    /// </summary>
    internal sealed class TyBuiltin
    {
        private readonly TokenBuiltin name;

        public Span span => name.span;
        public string Name => name.image;

        public TyBuiltin(TokenBuiltin name)
        {
            this.name = name;
        }
    }
}
