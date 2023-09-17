using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kapowey.Core.Entities
{
    [Table("user_user_role")]
    public partial class UserUserRole : IdentityUserRole<int>, IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => UserId;
            set => UserId = value;
        } 
        
        [Key]
        [Column("user_id")]
        public override int UserId { get; set; }

        [Key]
        [Column("user_role_id")]
        public override int RoleId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserUserRole")]
        public virtual User User { get; set; }

        [ForeignKey(nameof(RoleId))]
        [InverseProperty("UserUserRole")]
        public virtual UserRole UserRole { get; set; }
    }
}