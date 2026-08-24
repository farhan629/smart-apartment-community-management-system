using IdentityService.Application.Features.Roles.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Roles.Queries
{
    /// <summary>
    /// Query to retrieve a role by its unique identifier.
    /// </summary>
    public class GetRoleByIdQuery : IRequest<RoleDto>
    {
        /// <summary>Gets or sets the unique identifier of the role to fetch.</summary>
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Handler for processing the <see cref="GetRoleByIdQuery"/>.
    /// </summary>
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IRefTermRepository _refTermRepository;
        private readonly ILogger<GetRoleByIdQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRoleByIdQueryHandler"/> class.
        /// </summary>
        /// <param name="refTermRepository">The reference term repository.</param>
        /// <param name="logger">The logger instance.</param>
        public GetRoleByIdQueryHandler(
            IRefTermRepository refTermRepository,
            ILogger<GetRoleByIdQueryHandler> logger
        )
        {
            _refTermRepository = refTermRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the retrieval of a role by identifier.
        /// </summary>
        /// <param name="request">The query containing parameters.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A RoleDto containing details of the role.</returns>
        public async Task<RoleDto> Handle(
            GetRoleByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var term =
                await _refTermRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException(ExceptionMessages.NotFound);

            return new RoleDto
            {
                Id = term.Id,
                TermValue = term.Code,
                Description = term.DisplayName ?? string.Empty,
                Category = term.RefSet?.Code ?? string.Empty,
            };
        }
    }
}
