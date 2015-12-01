namespace Score.Front
{
    internal sealed class Spanned<T>
    {
        public Span span;
        public T value;

        public Spanned(Span span, T t)
        {
            this.span = span;
            this.value = t;
        }
    }
}
