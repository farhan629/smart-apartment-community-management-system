using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Response DTO detailing a successful operation outcome.
    /// </summary>
    public class SuccessResponseDto
    {
        /// <summary>Gets or sets a value indicating whether the operation was successful.</summary>
        public bool Success { get; set; } = true;

        /// <summary>Gets or sets an informational message about the outcome.</summary>
        public string Message { get; set; } = string.Empty;
    }
}