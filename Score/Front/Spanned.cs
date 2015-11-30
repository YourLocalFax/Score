namespace Score.Front
{
    internal sealed class Spanned<T>
    {
        public readonly Span span;
        public readonly T value;

        public Spanned(Span span, T t)
        {
            this.span = span;
            this.value = t;
        }
    }
}
