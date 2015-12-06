namespace Source
{
    /// <summary>
    /// A wrapper to provide span information about an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Spanned<T>
    {
        public Span span;
        public T value;

        public Spanned(Span span, T value)
        {
            this.span = span;
            this.value = value;
        }
    }
}
