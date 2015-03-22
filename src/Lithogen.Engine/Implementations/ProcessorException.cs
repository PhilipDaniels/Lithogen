using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// Represents an exception occuring during execution of a View Processor.
    /// </summary>
    [Serializable]
    public class ProcessorException : Exception
    {
        /// <summary>
        /// Construct a new ProcessorException.
        /// </summary>
        public ProcessorException()
        {
        }

        /// <summary>
        /// Construct a new ProcessorException.
        /// </summary>
        /// <param name="message">Message to use.</param>
        public ProcessorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construct a new ProcessorException with formatted message.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="args">Optional arguments for message.</param>
        public ProcessorException(string format, params object[] args)
            : base(String.Format(CultureInfo.InvariantCulture, format, args))
        {
        }

        /// <summary>
        /// Construct a new ProcessorException.
        /// </summary>
        /// <param name="message">Message to use.</param>
        /// <param name="innerException">Inner exception.</param>
        public ProcessorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Construct a new ProcessorException with formatted message.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="innerException">Inner exception.</param>
        /// <param name="args">Optional arguments for message.</param>
        public ProcessorException(string format, Exception innerException, params object[] args)
            : base(String.Format(CultureInfo.InvariantCulture, format, args), innerException)
        {
        }

        /// <summary>
        /// Construct a new ProcessorException using a serialization context.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaing context.</param>
        protected ProcessorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
