namespace Shared.SharedLibrary.Exceptions
{
    /// <summary>
    /// Represents an exception for bad request errors (HTTP 400).
    /// </summary>
    public class BadRequestException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message describing the exception.</param>
        public BadRequestException(string message)
            : base(message, 400) { }
    }
}
