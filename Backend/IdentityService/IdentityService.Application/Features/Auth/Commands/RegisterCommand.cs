using AutoMapper;
using IdentityService.Application.Features.Auth.DTOs;
using IdentityService.Application.Features.Auth.Validators;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<SuccessResponseDto>
    {
        public RegisterManagementRequestDto Request { get; set; } = null!;
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, SuccessResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefTermRepository _refTermRepository;
        private readonly IPasswordService _passwordService;
        private readonly IGrpcStaffClient _grpcStaffClient;
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IRefTermRepository refTermRepository,
            IPasswordService passwordService,
            IGrpcStaffClient grpcStaffClient,
            IMapper mapper,
            ILogger<RegisterCommandHandler> logger
        )
        {
            _userRepository = userRepository;
            _refTermRepository = refTermRepository;
            _passwordService = passwordService;
            _grpcStaffClient = grpcStaffClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SuccessResponseDto> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken
        )
        {
            var dto = request.Request;

            if (await _userRepository.EmailExistsAsync(dto.Email!.Trim()))
                throw new BadRequestException(ExceptionMessages.EmailAlreadyExists);

            if (dto.Role_id is null)
                throw new BadRequestException(ExceptionMessages.InvalidRole);

            var role =
                await _refTermRepository.GetByIdAsync(dto.Role_id.Value)
                ?? throw new BadRequestException(ExceptionMessages.InvalidRole);

            if (role.RefSetId != RefSetIds.ManagementSetId)
                throw new BadRequestException(ExceptionMessages.InvalidRole);

            bool isStaff = role.Code.Equals("Staff", StringComparison.OrdinalIgnoreCase);
            if (isStaff && dto.category_id is null)
                throw new BadRequestException(ExceptionMessages.StaffCategoryRequired);

            string? photoUrl = null;
            if (dto.Photo is not null && dto.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "staff"
                );
                Directory.CreateDirectory(uploadsFolder);

                var ext = Path.GetExtension(dto.Photo.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Photo.CopyToAsync(stream, cancellationToken);

                photoUrl = $"/uploads/staff/{fileName}";
            }

            var user = _mapper.Map<User>(dto);
            user.Id = Guid.NewGuid();
            user.Email = dto.Email.Trim();
            user.RoleId = role.Id;
            user.PhotoUrl = photoUrl;

            var credential = new UserPasswordSecurity
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                PasswordHash = _passwordService.HashPassword(dto.Password!),
            };

            await _userRepository.AddUserWithCredentialAsync(user, credential);

            if (isStaff)
            {
                _logger.LogInformation(
                    "Staff registered � propagating category {CategoryId} via gRPC to ComplaintMaintenanceService.",
                    dto.category_id
                );

                await _grpcStaffClient.CreateStaffAsync(user.Id, dto.category_id!.Value);
            }

            _logger.LogInformation(
                "Management registration successful for {Email}",
                dto.Email.Trim()
            );

            return new SuccessResponseDto { Message = "Staff/Admin registration successful." };
        }
    }
}
