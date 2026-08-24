using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for user operations.
/// </summary>
public class UserRepository : IdentityService.Application.Interfaces.Repositories.IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context
            .Users.AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNo = u.PhoneNo,
                PhotoUrl = u.PhotoUrl,
                RoleId = u.RoleId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Role =
                    u.Role == null
                        ? null
                        : new RefTerm
                        {
                            Id = u.Role.Id,
                            RefSetId = u.Role.RefSetId,
                            Code = u.Role.Code,
                            DisplayName = u.Role.DisplayName,
                        },
                FlatOccupancies = u.FlatOccupancies!.Select(fo => new FlatOccupancy
                    {
                        Id = fo.Id,
                        FlatId = fo.FlatId,
                        ResidentTypeId = fo.ResidentTypeId,
                        IsApproved = fo.IsApproved,
                        IsActive = fo.IsActive,
                    })
                    .ToList(),
            })
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var trimmedEmail = email.Trim();

        return await _context
            .Users.AsNoTracking()
            .Where(u => u.Email == trimmedEmail)
            .Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNo = u.PhoneNo,
                PhotoUrl = u.PhotoUrl,
                RoleId = u.RoleId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Role =
                    u.Role == null
                        ? null
                        : new RefTerm
                        {
                            Id = u.Role.Id,
                            RefSetId = u.Role.RefSetId,
                            Code = u.Role.Code,
                            DisplayName = u.Role.DisplayName,
                        },
            })
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByPhoneAsync(string phone)
    {
        var trimmedPhone = phone.Trim();

        return await _context
            .Users.AsNoTracking()
            .Where(u => u.PhoneNo == trimmedPhone)
            .Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNo = u.PhoneNo,
                PhotoUrl = u.PhotoUrl,
                RoleId = u.RoleId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Role =
                    u.Role == null
                        ? null
                        : new RefTerm
                        {
                            Id = u.Role.Id,
                            RefSetId = u.Role.RefSetId,
                            Code = u.Role.Code,
                            DisplayName = u.Role.DisplayName,
                        },
            })
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserWithCredentialAsync(string email)
    {
        var trimmedEmail = email.Trim();

        return await _context
            .Users.AsNoTracking()
            .Where(u => u.Email == trimmedEmail)
            .Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNo = u.PhoneNo,
                PhotoUrl = u.PhotoUrl,
                RoleId = u.RoleId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Role =
                    u.Role == null
                        ? null
                        : new RefTerm
                        {
                            Id = u.Role.Id,
                            RefSetId = u.Role.RefSetId,
                            Code = u.Role.Code,
                            DisplayName = u.Role.DisplayName,
                        },
                PasswordSecurity =
                    u.PasswordSecurity == null
                        ? null
                        : new UserPasswordSecurity
                        {
                            Id = u.PasswordSecurity.Id,
                            UserId = u.PasswordSecurity.UserId,
                            PasswordHash = u.PasswordSecurity.PasswordHash,
                        },
                FlatOccupancies = u.FlatOccupancies!.Select(fo => new FlatOccupancy
                    {
                        Id = fo.Id,
                        FlatId = fo.FlatId,
                        ResidentTypeId = fo.ResidentTypeId,
                        IsApproved = fo.IsApproved,
                        IsActive = fo.IsActive,
                    })
                    .ToList(),
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(int Total, IEnumerable<User> Items)> GetAllUsersAsync(
        int page,
        int limit,
        string? name = null,
        Guid? roleId = null
    )
    {
        Console.WriteLine("Name"+name);
        var query = _context.Users.AsNoTracking().Where(u => u.IsActive);

        if (!string.IsNullOrWhiteSpace(name))
        {
            var trimmedName = name.Trim().ToLower();
            query = query.Where(u => u.Name.ToLower().Contains(trimmedName));
        }

        if (roleId.HasValue)
        {
            query = query.Where(u => u.RoleId == roleId.Value);
        }

        query = query.OrderBy(u => u.CreatedAt);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNo = u.PhoneNo,
                PhotoUrl = u.PhotoUrl,
                RoleId = u.RoleId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Role =
                    u.Role == null
                        ? null
                        : new RefTerm
                        {
                            Id = u.Role.Id,
                            RefSetId = u.Role.RefSetId,
                            Code = u.Role.Code,
                            DisplayName = u.Role.DisplayName,
                        },
            })
            .ToListAsync();

        return (total, items);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var trimmedEmail = email.Trim();
        return await _context.Users.AnyAsync(u => u.Email == trimmedEmail);
    }

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<UserPasswordSecurity?> GetCredentialByUserIdAsync(Guid userId)
    {
        return await _context
            .UserPasswordSecurities.AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new UserPasswordSecurity
            {
                Id = c.Id,
                UserId = c.UserId,
                PasswordHash = c.PasswordHash,
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateCredentialAsync(UserPasswordSecurity credential)
    {
        _context.UserPasswordSecurities.Update(credential);
        await _context.SaveChangesAsync();
    }

    public async Task<User> AddUserWithCredentialAsync(User user, UserPasswordSecurity credential)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Users.AddAsync(user);
            await _context.UserPasswordSecurities.AddAsync(credential);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("User {UserId} created with credential", user.Id);
            return user;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> GetUserWithRoleAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNo = u.PhoneNo,
                PhotoUrl = u.PhotoUrl,
                RoleId = u.RoleId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Role =
                    u.Role == null
                        ? null
                        : new RefTerm
                        {
                            Id = u.Role.Id,
                            RefSetId = u.Role.RefSetId,
                            Code = u.Role.Code,
                            DisplayName = u.Role.DisplayName,
                        },
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<string>> GetRolePermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .RolePolicies.Where(rp => rp.RoleId == roleId && rp.IsAllowed)
            .Select(rp => rp.PermissionCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserPolicy>> GetUserPoliciesAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .UserPolicies.AsNoTracking()
            .Where(up => up.UserId == userId)
            .Select(up => new UserPolicy
            {
                Id = up.Id,
                UserId = up.UserId,
                PermissionCode = up.PermissionCode,
                IsAllowed = up.IsAllowed,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUsersByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Users.AsNoTracking()
            .Where(u => u.RoleId == roleId && u.IsActive)
            .Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNo = u.PhoneNo,
                PhotoUrl = u.PhotoUrl,
                RoleId = u.RoleId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Role =
                    u.Role == null
                        ? null
                        : new RefTerm
                        {
                            Id = u.Role.Id,
                            RefSetId = u.Role.RefSetId,
                            Code = u.Role.Code,
                            DisplayName = u.Role.DisplayName,
                        },
            })
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertUserPoliciesAsync(
        IEnumerable<UserPolicy> policies,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var policy in policies)
        {
            var existing = await _context.UserPolicies.FirstOrDefaultAsync(
                up => up.UserId == policy.UserId && up.PermissionCode == policy.PermissionCode,
                cancellationToken
            );

            if (existing != null)
            {
                existing.IsAllowed = policy.IsAllowed;
            }
            else
            {
                policy.Id = Guid.NewGuid();
                await _context.UserPolicies.AddAsync(policy, cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
