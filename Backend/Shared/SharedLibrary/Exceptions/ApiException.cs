namespace Shared.SharedLibrary.Exceptions
{
    /// <summary>
    /// Represents a custom exception used to handle API-related errors
    /// with an associated HTTP status code.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with the exception.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class
        /// /// with a specified error message and status code.
        /// </summary>
        /// <param name="message">The error message describing the exception.</param>
        /// <param name="statuscode">The HTTP status code associated with the error.</param>
        public ApiException(string message, int statuscode)
            : base(message)
        {
            StatusCode = statuscode;
        }
    }
}
