using AutoMapper;
using IdentityService.Application.Features.Users.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Application.Features.Users.Queries
{
    /// <summary>
    /// Query to retrieve a paginated list of all users, optionally filtered
    /// by name and/or role.
    /// </summary>
    public class GetAllUsersQuery : IRequest<PaginatedResponseDto<UserDto>>
    {
        /// <summary>Gets or sets the page number to fetch.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Gets or sets the page size limit.</summary>
        public int Limit { get; set; } = 10;

        /// <summary>Optional partial/full name to filter users by.</summary>
        public string? Name { get; set; }

        /// <summary>Optional role id to filter users by.</summary>
        public Guid? RoleId { get; set; }
    }

    /// <summary>
    /// Handler for processing the <see cref="GetAllUsersQuery"/>.
    /// </summary>
    public class GetAllUsersQueryHandler
        : IRequestHandler<GetAllUsersQuery, PaginatedResponseDto<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;
        private readonly IFlatOccupancyRepository _flatOccupancyrepository;

        public GetAllUsersQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<GetAllUsersQueryHandler> logger,
            IFlatOccupancyRepository flatOccupancyRepository
        )
        {
            _flatOccupancyrepository = flatOccupancyRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<UserDto>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken
        )
        {
            var pageNumber =
                request.Page < PaginationConstants.MinPageNumber
                    ? PaginationConstants.DefaultPageNumber
                    : request.Page;

            var pageSize =
                request.Limit < PaginationConstants.MinPageSize
                    ? PaginationConstants.DefaultPageSize
                    : request.Limit;

            if (pageSize > PaginationConstants.MaxPageSize)
            {
                pageSize = PaginationConstants.MaxPageSize;
            }

            var (total, users) = await _userRepository.GetAllUsersAsync(
                pageNumber,
                pageSize,
                request.Name,
                request.RoleId
            );

            List<UserDto> userList = new List<UserDto>();
            foreach (User u in users)
            {
                var flat_id = await _flatOccupancyrepository.getUserIdFlat(u.Id);
                UserDto result = _mapper.Map<UserDto>(u);
                result.FlatId = flat_id;
                userList.Add(result);
            }

            _logger.LogInformation(
                "Fetched page {Page} of users ({Limit}/page), Name={Name}, RoleId={RoleId}",
                pageNumber,
                pageSize,
                request.Name,
                request.RoleId
            );

            return new PaginatedResponseDto<UserDto>
            {
                Page = pageNumber,
                Limit = pageSize,
                Total = total,
                Items = userList,
            };
        }
    }
}