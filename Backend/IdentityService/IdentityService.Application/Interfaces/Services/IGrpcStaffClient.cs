using System;
using System.Threading.Tasks;

namespace IdentityService.Application.Interfaces.Services
{
    /// <summary>
    /// gRPC client interface for communicating with the Staff service.
    /// </summary>
    public interface IGrpcStaffClient
    {
        /// <summary>
        /// Asynchronously sends a request to create a staff record in the Staff service.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="categoryId">The unique identifier of the staff category.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateStaffAsync(Guid userId, Guid categoryId);
    }
}