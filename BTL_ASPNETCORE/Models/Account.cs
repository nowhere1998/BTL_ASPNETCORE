using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_ASPNETCORE.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        [Required]
        [StringLength(100)]
        public string? Password { get; set; }
        [Required]
        [StringLength(200)]
        public string? Email { get; set; }
        public string? Phone {  get; set; }
        public int Role { get; set; } = 0;
        public string? Image {  get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
