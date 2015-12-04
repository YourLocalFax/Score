using System.Collections.Generic;
using System.IO;

using Source;

namespace Log
{
    /// <summary>
    /// This is a single detail for the log.
    /// TODO(kai): describe this better.
    /// </summary>
    internal sealed class Detail
    {
        /// <summary>
        /// The possible types of a detail.
        /// </summary>
        public enum Type
        {
            INFO,
            WARN,
            ERROR,
        }

        /// <summary>
        /// The type of this detail.
        /// </summary>
        public readonly Type type;
        /// <summary>
        /// This details (hopefully) descriptive message.
        /// TODO(kai): Figure out a way to easily provide formatted messages.
        /// </summary>
        public readonly string message;

        public Detail(Type type, string message)
        {
            this.type = type;
            this.message = message;
        }
    }

    /// <summary>
    /// This is the detail logger
    /// TODO(kai): describe this better, please.
    /// </summary>
    public sealed class DetailLogger
    {
        private readonly List<Spanned<Detail>> details = new List<Spanned<Detail>>();

        private uint errorCount;
        public bool HasErrors => errorCount != 0;

        public DetailLogger()
        {
        }

        // TODO(kai): allow a filter for only writing certain kinds of details.
        public void Print(TextWriter writer)
        {
            details.ForEach(detail =>
            {
                writer.Write(detail.span);
                writer.WriteLine(':');
                writer.WriteLine(detail.value.message);
                writer.WriteLine();
            });
        }

        /// <summary>
        /// Adds a warning message to this logger.
        /// </summary>
        /// <param name="span">Where in the source file this warning describes</param>
        /// <param name="format">The detail message</param>
        /// <param name="args">The arguments used to format the message</param>
        public void Warn(Span span, string format, params object[] args) =>
            details.Add(new Detail(Detail.Type.WARN, string.Format(format, args)).Spanned(span));

        /// <summary>
        /// Adds a warning message to this logger.
        /// </summary>
        /// <param name="span">Where in the source file this warning describes</param>
        /// <param name="message">The detail message</param>
        public void Warn(Span span, string message) =>
            details.Add(new Detail(Detail.Type.WARN, message).Spanned(span));

        /// <summary>
        /// Adds an error message to this logger.
        /// 
        /// Error messages prevent subsequent steps from executing.
        /// For example, if the lexing stage produces errors, the parsing
        /// stage will not occur, and the compilation fails.
        /// </summary>
        /// <param name="span">Where in the source file this error describes</param>
        /// <param name="format">The detail message</param>
        /// <param name="args">The arguments used to format the message</param>
        public void Error(Span span, string format, params object[] args)
        {
            errorCount++;
            details.Add(new Detail(Detail.Type.ERROR, string.Format(format, args)).Spanned(span));
        }

        /// <summary>
        /// Adds an error message to this logger.
        /// 
        /// Error messages prevent subsequent steps from executing.
        /// For example, if the lexing stage produces errors, the parsing
        /// stage will not occur, and the compilation fails.
        /// </summary>
        /// <param name="span">Where in the source file this error describes</param>
        /// <param name="message">The detail message</param>
        public void Error(Span span, string message)
        {
            errorCount++;
            details.Add(new Detail(Detail.Type.ERROR, message).Spanned(span));
        }
    }
}
