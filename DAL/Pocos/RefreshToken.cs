using JWT.Areas.Identity.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT.Areas.Identity.Data
{
    public class RefreshToken: IWithUserId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Token { get; set; }
        public string JwtId { get; set; } = "";
        public bool Used { get; set; }
        public bool Invalidated { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        // Relationships
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
