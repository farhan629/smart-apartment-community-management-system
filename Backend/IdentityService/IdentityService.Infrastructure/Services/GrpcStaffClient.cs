using System;
using System.Threading.Tasks;
using ComplaintMaintenanceService.Infrastructure.Protos;
using Grpc.Net.Client;
using IdentityService.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for the gRPC client that communicates with the Staff/Complaint maintenance service.
    /// </summary>
    public class GrpcStaffClient : IGrpcStaffClient
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GrpcStaffClient> _logger;

        public GrpcStaffClient(IConfiguration configuration, ILogger<GrpcStaffClient> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task CreateStaffAsync(Guid userId, Guid categoryId)
        {
            var serviceUrl =
                _configuration[ConfigKeys.GrpcComplaintServiceUrl]
                ?? ConfigDefaults.ComplaintServiceUrl;

            _logger.LogInformation(
                "Connecting to ComplaintMaintenanceService gRPC server at {Url}",
                serviceUrl
            );

            using var channel = GrpcChannel.ForAddress(serviceUrl);
            var client = new CmsService.CmsServiceClient(channel);

            var request = new CreateStaffRequest
            {
                UserId = userId.ToString(),
                CategoryId = categoryId.ToString(),
            };

            var response = await client.CreateStaffAsync(request);

            _logger.LogInformation(
                "Successfully created staff profile for User {UserId} in ComplaintService. Staff ID: {StaffId}",
                userId,
                response.StaffId
            );
        }
    }
}
