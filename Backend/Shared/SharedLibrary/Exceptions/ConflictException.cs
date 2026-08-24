namespace Shared.SharedLibrary.Exceptions
{
    /// <summary>
    /// Represents an exception for conflict errors (HTTP 409).
    /// </summary>
    public class ConflictException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message describing the exception.</param>
        public ConflictException(string message)
            : base(message, 409) { }
    }
}
