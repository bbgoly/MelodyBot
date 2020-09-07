using System;
using System.Runtime.Serialization;

namespace MelodyBot.Exceptions
{
    /// <summary>
    /// Thrown whenever a command fails to execute
    /// </summary>
    [Serializable]
    public class CommandFailedException : Exception
    {
        /// <summary>
        /// Throws an exception with the default reason
        /// </summary>
        public CommandFailedException() : base("Command execution did not meet the requirements") { }

        /// <summary>
        /// Throws an exception with a specified reason
        /// </summary>
        /// <param name="message">Exception reason</param>
        public CommandFailedException(string message) : base(message) { }

        /// <summary>
        /// Throws an exception with a specified reason and specified cause
        /// </summary>
        /// <param name="message">Exception reason</param>
        /// <param name="innerException">Source of error</param>
        public CommandFailedException(string message, Exception innerException) : base(message, innerException) { }

        protected CommandFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}