namespace Source
{
    public static class Util
    {
        /// <summary>
        /// Wraps this object with the given span.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static Spanned<T> Spanned<T>(this T value, Span span) =>
            new Spanned<T>(span, value);
    }
}
