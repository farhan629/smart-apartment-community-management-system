using AutoMapper;
using IdentityService.Application.Features.Users.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Users.Queries
{
    /// <summary>
    /// Query to retrieve a single user by their unique identifier.
    /// </summary>
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        /// <summary>Gets or sets the unique identifier of the user to retrieve.</summary>
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Handler for processing the <see cref="GetUserByIdQuery"/>.
    /// </summary>
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFlatOccupancyRepository _flatOccupancyrepository;

        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserByIdQueryHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public GetUserByIdQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            IFlatOccupancyRepository flatOccupancyRepository,
            ILogger<GetUserByIdQueryHandler> logger
        )
        {
            _flatOccupancyrepository = flatOccupancyRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the retrieval of a user by identifier.
        /// </summary>
        /// <param name="request">The query containing parameters.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A UserDto containing user details.</returns>
        public async Task<UserDto> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var user =
                await _userRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException(ExceptionMessages.NotFound);
            var flat_id = await _flatOccupancyrepository.getUserIdFlat(user.Id);
            _logger.LogInformation("Fetched user {UserId}", request.Id);
            UserDto result = _mapper.Map<UserDto>(user);
            result.FlatId = flat_id;
            return result;
        }
    }
}
