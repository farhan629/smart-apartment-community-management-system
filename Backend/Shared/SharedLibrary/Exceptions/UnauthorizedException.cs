namespace Shared.SharedLibrary.Exceptions
{
    /// <summary>
    /// Represents an exception for unauthorized access errors (HTTP 401).
    /// </summary>
    public class UnauthorizedException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message describing the exception.</param>
        public UnauthorizedException(string message)
            : base(message, 401) { }
    }
}
