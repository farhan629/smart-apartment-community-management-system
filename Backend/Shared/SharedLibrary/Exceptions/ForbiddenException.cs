namespace Shared.SharedLibrary.Exceptions
{
    /// <summary>
    /// Represents an exception for forbidden access errors (HTTP 403).
    /// </summary>
    public class ForbiddenException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message describing the exception.</param>
        public ForbiddenException(string message)
            : base(message, 403) { }
    }
}
