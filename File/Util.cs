namespace Source
{
    public static class Util
    {
        public static Spanned<T> Spanned<T>(this T obj, Span span) =>
            new Spanned<T>(span, obj);
    }
}
