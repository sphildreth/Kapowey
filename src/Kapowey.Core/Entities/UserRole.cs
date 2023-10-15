using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kapowey.Core.Entities
{
    [Table("user_role")]
    public partial class UserRole : IdentityRole<int>, IEntity<int>
    {
        public UserRole()
        {
            UserUserRole = new HashSet<UserUserRole>();
        }

        [NotMapped]
        public int UserRoleId => Id;

        [Column("name")]
        [StringLength(256)]
        public override string Name { get; set; }

        [Column("normalized_name")]
        [StringLength(256)]
        public override string NormalizedName { get; set; }

        [Column("concurrency_stamp")]
        public override string? ConcurrencyStamp { get; set; }

        public virtual ICollection<UserRoleClaim> Claims { get; set; }

        [InverseProperty("UserRole")]
        public virtual ICollection<UserUserRole> UserUserRole { get; set; }
    }
}