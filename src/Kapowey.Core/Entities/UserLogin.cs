using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kapowey.Core.Entities
{
    [Table("user_login")]
    public partial class UserLogin : IdentityUserLogin<int>, IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => UserId;
            set => UserId = value;
        } 
        
        [Key]
        [Column("login_provider")]
        [StringLength(128)]
        public override string LoginProvider { get; set; } = default!;

        [Key]
        [Column("provider_key")]
        [StringLength(128)]
        public override string ProviderKey { get; set; } = default!;

        [Column("provider_display_name")]
        public override string? ProviderDisplayName { get; set; }

        [Column("user_id")]
        public override int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserLogin")]
        public virtual User User { get; set; }
    }
}