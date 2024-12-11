using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_ASPNETCORE.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   
        public int OrderId { get; set; }
        public int AccountId { get; set; }
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public float TotalPrice {  get; set; }
        public int status { get; set; } = 0;
        public string? note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [ForeignKey("AccountId")]
        public Account? Account { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    }
}
