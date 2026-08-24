using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Shared.SharedLibrary.DTO;

namespace IdentityService.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public  User User { get; set; }
        public string TokenKey { get; set; }
        public DateTime ExpiryAt { get; set; }

    }
}