using IdentityService.Application.Features.Flats.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Flats.Queries
{
    /// <summary>
    /// Query used to retrieve a flat by its unique identifier.
    /// </summary>
    public class GetFlatByIdQuery : IRequest<FlatResponseDto>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the flat.
        /// </summary>
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Handles the retrieval of a flat by its unique identifier.
    /// </summary>
    public class GetFlatByIdQueryHandler : IRequestHandler<GetFlatByIdQuery, FlatResponseDto>
    {
        private readonly IFlatRepository _flatRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFlatByIdQueryHandler"/> class.
        /// </summary>
        /// <param name="flatRepository">
        /// Repository used to access flat data.
        /// </param>
        public GetFlatByIdQueryHandler(IFlatRepository flatRepository)
        {
            _flatRepository = flatRepository;
        }

        /// <summary>
        /// Retrieves a flat by its identifier and returns its details.
        /// </summary>
        /// <param name="request">
        /// The query containing the flat identifier.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A <see cref="FlatResponseDto"/> containing the flat details.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no flat exists with the specified identifier.
        /// </exception>
        public async Task<FlatResponseDto> Handle(
            GetFlatByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var flat = await _flatRepository.GetByIdAsync(request.Id, cancellationToken);

            if (flat is null)
                throw new NotFoundException(ExceptionMessages.NotFound);

            return new FlatResponseDto
            {
                Id = flat.Id,
                Number = flat.Number,
                Block = flat.Block,
                Floor = flat.Floor,
                IsAvailable = !(flat.FlatOccupancies?.Any(o => o.FlatId == flat.Id) ?? false),
                CreatedAt = flat.CreatedAt,
            };
        }
    }
}
