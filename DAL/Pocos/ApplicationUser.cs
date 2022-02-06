using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JWT.Areas.Identity.Interfaces;

namespace JWT.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser<Guid>, IWithId<Guid>
    {
        [Required]
        [StringLength(256)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(256)]
        public string LastName { get; set; }

        // Relationships
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
