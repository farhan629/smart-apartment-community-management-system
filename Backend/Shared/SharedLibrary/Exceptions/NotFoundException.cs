namespace Shared.SharedLibrary.Exceptions
{
    /// <summary>
    /// Represents an exception for resource not found errors (HTTP 404).
    /// </summary>
    public class NotFoundException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message describing the exception.</param>
        public NotFoundException(string message)
            : base(message, 404) { }
    }
}
