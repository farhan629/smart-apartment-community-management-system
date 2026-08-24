using AutoMapper;
using IdentityService.Application.Features.Roles.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Roles.Queries
{
    /// <summary>
    /// Query to retrieve all roles matching a specific category.
    /// </summary>
    public class GetRolesQuery : IRequest<IEnumerable<RoleDto>>
    {
        /// <summary>Gets or sets the category of roles to retrieve ("Occupant" or "Management").</summary>
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    /// Handler for processing the <see cref="GetRolesQuery"/>.
    /// </summary>
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IEnumerable<RoleDto>>
    {
        private readonly IRefSetRepository _refSetRepository;
        private readonly IRefTermRepository _refTermRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRolesQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRolesQueryHandler"/> class.
        /// </summary>
        /// <param name="refSetRepository">The reference set repository.</param>
        /// <param name="refTermRepository">The reference term repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public GetRolesQueryHandler(
            IRefSetRepository refSetRepository,
            IRefTermRepository refTermRepository,
            IMapper mapper,
            ILogger<GetRolesQueryHandler> logger
        )
        {
            _refSetRepository = refSetRepository;
            _refTermRepository = refTermRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the retrieval of roles by category.
        /// </summary>
        /// <param name="request">The query containing parameters.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A collection of RoleDto objects matching the requested category.</returns>
        public async Task<IEnumerable<RoleDto>> Handle(
            GetRolesQuery request,
            CancellationToken cancellationToken
        )
        {
            var refSet =
                await _refSetRepository.GetBySetNameAsync(request.Category)
                ?? throw new NotFoundException(ExceptionMessages.NotFound);

            var terms = await _refTermRepository.GetByRefSetIdAsync(refSet.Id);

            _logger.LogInformation(
                "Fetched {Count} roles for category '{Category}'",
                terms.Count(),
                request.Category
            );

            return terms.Select(t => new RoleDto
            {
                Id = t.Id,
                TermValue = t.Code,
                Description = t.DisplayName ?? string.Empty,
                Category = request.Category,
            });
        }
    }
}
