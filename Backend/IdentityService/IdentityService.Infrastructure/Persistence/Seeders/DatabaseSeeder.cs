using System;
using System.Collections.Generic;
using System.Globalization;
using CsvHelper;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Infrastructure.Seeders
{
    /// <summary>
    /// Orchestrates and seeds database from embedded CSV files (ref_sets, ref_terms, flats).
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly IRefSetRepository _refSetRepository;
        private readonly IRefTermRepository _refTermRepository;
        private readonly IFlatRepository _flatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSeeder"/> class.
        /// </summary>
        /// <param name="refSetRepository">The reference set repository.</param>
        /// <param name="refTermRepository">The reference term repository.</param>
        /// <param name="flatRepository">The flat repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="passwordService">The password security service.</param>
        /// <param name="context">The database context for direct DbSet access.</param>
        /// <param name="logger">The logger instance.</param>
        public DatabaseSeeder(
            IRefSetRepository refSetRepository,
            IRefTermRepository refTermRepository,
            IFlatRepository flatRepository,
            IUserRepository userRepository,
            IPasswordService passwordService,
            AppDbContext context,
            ILogger<DatabaseSeeder> logger
        )
        {
            _refSetRepository = refSetRepository;
            _refTermRepository = refTermRepository;
            _flatRepository = flatRepository;
            _userRepository = userRepository;
            _passwordService = passwordService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously seeds all initial seed data into the database.
        /// </summary>
        /// <returns>A task representing the seeding operation.</returns>
        public async Task SeedAsync()
        {
            _logger.LogInformation("=== Database seeding started ===");

            // 1. Seed RefSets
            await SeedRefSetsAsync();

            // 2. Seed RefTerms
            await SeedRefTermsAsync();

            // 3. Seed Users
            await SeedUsersAsync();

            // 4. Seed Flats
            await SeedFlatsAsync();

            // 5. Seed RolePolicies
            await SeedRolePoliciesAsync();

            _logger.LogInformation("=== Database seeding completed ===");
        }

        /// <summary>
        /// Opens an embedded CSV resource and returns its rows as dynamic records,
        /// keyed by column header name (avoids per-file DTO classes).
        /// </summary>
        private static List<dynamic> ReadEmbeddedCsv(string resourceName)
        {
            var assembly = typeof(DatabaseSeeder).Assembly;

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
            }

            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<dynamic>().ToList();
        }

        private async Task SeedRefSetsAsync()
        {
            var records = ReadEmbeddedCsv(SeedDataResourceNames.RefSets);

            foreach (var r in records)
            {
                Guid id = Guid.Parse((string)r.Id);
                string setName = ((string)r.SetName).Trim();
                string description = ((string)r.Description).Trim();

                var existing = await _refSetRepository.GetBySetNameAsync(setName);
                if (existing is not null)
                {
                    _logger.LogInformation(
                        "RefSet '{SetName}' already exists — skipping.",
                        setName
                    );
                    continue;
                }

                var refSet = new RefSet
                {
                    Id = id,
                    Code = setName,
                    Description = description,
                };

                try
                {
                    await _refSetRepository.AddAsync(refSet);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Seeded RefSet '{SetName}'.", setName);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "Skipping RefSet '{SetName}' — already exists or constraint violation: {Message}",
                        setName,
                        ex.Message
                    );
                    _context.Entry(refSet).State = EntityState.Detached;
                }
            }
        }

        private async Task SeedRefTermsAsync()
        {
            var records = ReadEmbeddedCsv(SeedDataResourceNames.RefTerms);

            foreach (var r in records)
            {
                Guid id = Guid.Parse((string)r.Id);
                string termValue = ((string)r.TermValue).Trim();
                string description = ((string)r.Description).Trim();
                Guid refSetId = Guid.Parse((string)r.RefSetId);

                var existing = await _refTermRepository.GetByTermValueAndSetIdAsync(
                    termValue,
                    refSetId
                );
                if (existing is not null)
                {
                    _logger.LogInformation(
                        "RefTerm '{TermValue}' already exists — skipping.",
                        termValue
                    );
                    continue;
                }

                var refTerm = new RefTerm
                {
                    Id = id,
                    Code = termValue,
                    DisplayName = description,
                    RefSetId = refSetId,
                };

                try
                {
                    await _refTermRepository.AddAsync(refTerm);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Seeded RefTerm '{TermValue}'.", termValue);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "Skipping RefTerm '{TermValue}' — already exists or constraint violation: {Message}",
                        termValue,
                        ex.Message
                    );
                    _context.Entry(refTerm).State = EntityState.Detached;
                }
            }
        }

        private async Task SeedFlatsAsync()
        {
            var records = ReadEmbeddedCsv(SeedDataResourceNames.Flats);

            var seededCount = 0;

            foreach (var r in records)
            {
                string number = ((string)r.Number).Trim();
                string block = ((string)r.Block).Trim();
                int floor = int.Parse((string)r.Floor, CultureInfo.InvariantCulture);

                var existing = await _flatRepository.GetByNumberAndBlockAsync(number, block);
                if (existing is not null)
                {
                    _logger.LogInformation(
                        "Flat '{Number}' in Block '{Block}' already exists — skipping.",
                        number,
                        block
                    );
                    continue;
                }

                var flat = new Flat
                {
                    Id = Guid.NewGuid(),
                    Number = number,
                    Block = block,
                    Floor = floor,
                };

                try
                {
                    await _flatRepository.AddAsync(flat);
                    await _context.SaveChangesAsync();
                    seededCount++;
                    _logger.LogInformation(
                        "Seeded Flat '{Number}' in Block '{Block}'.",
                        number,
                        block
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "Skipping Flat '{Number}' in Block '{Block}' — already exists or constraint violation: {Message}",
                        number,
                        block,
                        ex.Message
                    );
                    _context.Entry(flat).State = EntityState.Detached;
                }
            }

            _logger.LogInformation("Seeded {Count} new flats.", seededCount);
        }

        private async Task SeedUsersAsync()
        {
            var records = ReadEmbeddedCsv(SeedDataResourceNames.Users);

            foreach (var r in records)
            {
                Guid id = Guid.Parse((string)r.Id);
                string name = ((string)r.Name).Trim();
                string email = ((string)r.Email).Trim();
                string phone = ((string)r.Phone).Trim();
                string? photoUrlRaw = (string?)r.PhotoUrl;
                string? photoUrl = string.IsNullOrWhiteSpace(photoUrlRaw)
                    ? null
                    : photoUrlRaw!.Trim();
                Guid roleId = Guid.Parse((string)r.RoleId);
                string password = ((string)r.Password).Trim();

                var existing = await _userRepository.EmailExistsAsync(email);
                if (existing)
                {
                    _logger.LogInformation(
                        "User with email '{Email}' already exists — updating password hash.",
                        email
                    );
                    var userObj = await _userRepository.GetByEmailAsync(email);
                    if (userObj != null)
                    {
                        var cred = await _userRepository.GetCredentialByUserIdAsync(userObj.Id);
                        if (cred != null)
                        {
                            cred.PasswordHash = _passwordService.HashPassword(password);
                            await _userRepository.UpdateCredentialAsync(cred);
                        }
                    }
                    continue;
                }

                var user = new User
                {
                    Id = id,
                    Name = name,
                    Email = email,
                    PhoneNo = phone,
                    PhotoUrl = photoUrl,
                    RoleId = roleId,
                };

                var credential = new UserPasswordSecurity
                {
                    Id = Guid.NewGuid(),
                    UserId = id,
                    PasswordHash = _passwordService.HashPassword(password),
                };

                try
                {
                    await _userRepository.AddUserWithCredentialAsync(user, credential);

                    // Force the insert now so a duplicate-key violation surfaces
                    // here (and is skipped) instead of being deferred and thrown
                    // later inside an unrelated SaveChangesAsync call (e.g. RolePolicies).
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "Seeded User '{Email}' with Role '{RoleId}'.",
                        email,
                        roleId
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "Skipping user '{Email}' — already exists or constraint violation: {Message}",
                        email,
                        ex.Message
                    );

                    // Detach the entities that failed to save so they don't stay
                    // "Added" in the tracker and get retried on the next SaveChanges.
                    _context.Entry(user).State = EntityState.Detached;
                    _context.Entry(credential).State = EntityState.Detached;
                }
            }
        }

        private async Task SeedRolePoliciesAsync()
        {
            var records = ReadEmbeddedCsv(SeedDataResourceNames.RolePolicies);

            foreach (var r in records)
            {
                Guid id = Guid.Parse((string)r.Id);
                Guid roleId = Guid.Parse((string)r.RoleId);
                string permissionCode = (string)r.PermissionCode;
                string description = (string)r.Description;
                bool isAllowed = bool.Parse((string)r.IsAllowed);

                var alreadyExists = _context.RolePolicies.Any(rp =>
                    rp.RoleId == roleId && rp.PermissionCode == permissionCode
                );

                if (alreadyExists)
                {
                    _logger.LogInformation(
                        "RolePolicy '{PermissionCode}' for role '{RoleId}' already exists — skipping.",
                        permissionCode,
                        roleId
                    );
                    continue;
                }

                _context.RolePolicies.Add(
                    new RolePolicy
                    {
                        Id = id,
                        RoleId = roleId,
                        PermissionCode = permissionCode,
                        Description = description,
                        IsAllowed = isAllowed,
                    }
                );

                _logger.LogInformation(
                    "Seeded RolePolicy '{PermissionCode}' for role '{RoleId}'.",
                    permissionCode,
                    roleId
                );
            }

            await _context.SaveChangesAsync();
        }
    }
}
